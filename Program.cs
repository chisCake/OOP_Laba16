using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using AngleSharp;
using AngleSharp.Io;
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable AccessToModifiedClosure

namespace OOP_Laba16 {
	class Program {
		static readonly List<Action> Tasks = new List<Action> {
			Task1,
			Task2,
			Task3_4,
			Task5,
			Task6,
			Task7,
			Task8
		};

		static void Main() {
			while (true) {
				Console.Write(
					"1 - длительная по времени задача" +
					"\n2 - с токеном отмены" +
					"\n3 - задача продолжения" +
					"\n4 - распаралеллить вычисление циклов" +
					"\n5 - распараллелить выполнение блока операторов" +
					"\n6 - склад с BlockingCollection" +
					"\n7 - async await" +
					"\n0 - выход" +
					"\nВыберите действие: "
					);
				if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > Tasks.Count) {
					Console.WriteLine("Нет такого действия");
					Console.ReadKey();
					Console.Clear();
					continue;
				}
				if (choice == 0) {
					Console.WriteLine("Выход...");
					Environment.Exit(0);
				}
				Tasks[choice - 1]();
				Console.ReadKey();
				Console.Clear();
			}
		}

		static void Task1() {
			var task = new Task(() => PrimeNumbers.Eratosfen(250000));
			task.Start();
			Console.WriteLine("ID текущей задачи: " + task.Id);

			int counter = 1;
			int interval = 1000;
			do {
				Thread.Sleep(interval);
				Console.WriteLine($"Прошло {counter++ * interval / 1000} сек, задача ещё не выполнена");
			} while (task.Status == TaskStatus.Running);
			Console.WriteLine("Выполнение задачи завершено");


			int run = 5;
			int max = 100000;

			long sumE = 0, sumS = 0;

			Console.WriteLine($"Подсчёт производительности на нескольких прогонах ({run}), с максимальным числом {max} (мс)");
			var sw = new Stopwatch();
			Console.WriteLine("Эратосфен    Не Эратосфен");
			for (int i = 0; i < run; i++) {
				sw.Start();
				task = new Task(() => PrimeNumbers.Eratosfen(max));
				task.Start();
				task.Wait();
				sw.Stop();
				long elE = sw.ElapsedMilliseconds;
				sumE += elE;

				sw.Reset();

				sw.Start();
				task = new Task(() => PrimeNumbers.Simple(max));
				task.Start();
				task.Wait();
				sw.Stop();
				long elS = sw.ElapsedMilliseconds;
				sumS += elS;

				Console.WriteLine($"{elE,6}{elS,15}");
			}
			Console.WriteLine($"Среднее время:\nЭратосфен: {sumE / run}мс\nПоследовательный алгоритм: {sumS / run}мс");
		}

		static void Task2() {
			var eratosfen = new Action<object, CancellationToken>(PrimeNumbers.Eratosfen);

			var tokenSrc = new CancellationTokenSource();
			var token = tokenSrc.Token;

			var task = new Task(() => eratosfen(1000000, token));
			task.Start();
			Console.WriteLine("ID текущей задачи: " + task.Id);

			int counter = 1;
			int interval = 1000;
			do {
				Thread.Sleep(interval);
				Console.WriteLine($"Прошло {counter * interval / 1000} сек, задача ещё не выполнена");
				if (counter++ == 5) {
					tokenSrc.Cancel();
					Console.WriteLine("Отмена");
					break;
				}
			} while (task.Status == TaskStatus.Running);
			Console.WriteLine("Выполнение задачи завершено");
		}

		static void Task3_4() {
			var func = new Func<int, int>(x => { Thread.Sleep(1000 * x); return x + 1; });

			Console.WriteLine("ContinueWith");
			var task1 = new Task<int>(() => func(1));
			task1.Start();
			var task2 = task1.ContinueWith(task => func(task.Result));
			var task3 = task2.ContinueWith(task => func(task.Result));
			task3.Wait();
			Console.WriteLine("Полученное число: " + task3.Result);

			Console.WriteLine("Awaiter");
			task1 = new Task<int>(() => func(1));
			task1.Start();
			var res = task1.GetAwaiter().GetResult();
			task2 = new Task<int>(() => func(res));
			task2.Start();
			res = task2.GetAwaiter().GetResult();
			task3 = new Task<int>(() => func(res));
			task3.Start();
			res = task3.GetAwaiter().GetResult();
			Console.WriteLine("Полученное число: " + res);
		}

		static void Task5() {
			int run = 5, amt = 10000000;

			var list = new List<double>();
			for (int i = 0; i < amt; i++)
				list.Add(i);

			long sumP = 0, sumS = 0;

			Console.WriteLine($"Подсчёт производительности на нескольких прогонах ({run}), при кол-во элементов {amt} (мс)");
			var sw = new Stopwatch();
			Console.WriteLine("Параллельно    Последовательно");
			for (int i = 0; i < run; i++) {
				sw.Start();
				Parallel.ForEach(list, DoSmth);
				sw.Stop();
				long elP = sw.ElapsedMilliseconds;
				sumP += elP;

				sw.Reset();

				sw.Start();
				list.ForEach(DoSmth);
				sw.Stop();
				long elS = sw.ElapsedMilliseconds;
				sumS += elS;

				Console.WriteLine($"{elP,6}{elS,17}");
			}
			Console.WriteLine($"Среднее время:\nПараллельно: {sumP / run}мс\nПоследовательно: {sumS / run}мс");

			static void DoSmth(double x) => Math.Sin(x);
		}

		static void Task6() {
			Parallel.Invoke(DoSmth, () => {
				for (int i = 0; i < 5; i++) {
					Console.WriteLine("Что-то происходит в первой задаче");
					Thread.Sleep(1000);
				}
			});

			static void DoSmth() {
				for (int i = 0; i < 10; i++) {
					Console.WriteLine("Что-то происходит во второй задаче");
					Thread.Sleep(500);
				}
			}
		}

		static void Task7() {
			var storage = new Storage();
			var suppliers = new List<Supplier>() {
				new Supplier("Телевизор", 2000, storage),
				new Supplier("Компьютер", 3000, storage),
				new Supplier("Холодильник", 2500, storage),
				new Supplier("Микроволновка", 4000, storage),
				new Supplier("Пылесос", 5000, storage)
			};
			var consumers = new List<Consumer>();
			var rndm = new Random();
			for (int i = 0; i < 10; i++)
				consumers.Add(new Consumer(rndm.Next(5000, 10000), storage));

			suppliers.ForEach(item => item.StartDeliveries());
			consumers.ForEach(item => item.StartBuying());

			Console.ReadKey();
			suppliers.ForEach(item => item.FinishDeliveries());
			consumers.ForEach(item => item.FinishBuying());
		}

		static void Task8() {
			Console.WriteLine("Запрос времени...");
			try {
				Console.WriteLine("Время в Минске: " + GetTimeAsync().Result);
			}
			catch (Exception e) {
				Console.WriteLine($"Не удалось запросить время\n{e.Message}");
			}
		}

		static async Task<string> GetTimeAsync() {
			var requester = new DefaultHttpRequester();
			requester.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";
			var config = Configuration.Default.With(requester).WithDefaultLoader();

			var document = await BrowsingContext.New(config).OpenAsync("https://time.is/ru/Minsk");
			return document.GetElementById("clock").TextContent;
		}
	}
}