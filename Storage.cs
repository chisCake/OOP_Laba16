using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OOP_Laba16
{
	class Storage
	{
		public Storage()
		{
			Products = new BlockingCollection<Product>();
		}

		public BlockingCollection<Product> Products { get; private set; }

		public void Add(Product product)
		{
			Products.Add(product);
			Console.WriteLine($"+ {product.Name}\n");
			PrintProducts();
		}

		public Product Take()
		{
			var product = Products.Take();
			if (product != null)
			{
				Console.WriteLine($"- {product.Name}\n");
				PrintProducts();
			}

			return product;
		}

		public void PrintProducts()
		{
			Console.Clear();
			foreach (var item in Products.OrderBy(item => item.Id))
				Console.WriteLine($"{item.Name,-20} {item.Id}");
		}
	}

	class Product
	{
		static readonly Random Rdnm = new Random();

		public Product(string name)
		{
			Name = name;
			Id = Rdnm.Next(100000, 1000000);
		}

		public string Name { get; }
		public int Id { get; }

		public override string ToString() => $"{Name} {Id}";
	}

	class Supplier
	{
		Task _task;

		public Supplier(string product, int interval, params Storage[] storages)
		{
			Product = product;
			Interval = interval;
			Storages = new List<Storage>(storages);
		}

		public string Product { get; set; }
		public int Interval { get; set; }
		public List<Storage> Storages { get; private set; }
		public bool NowSupplies { get; private set; }

		public void StartDeliveries()
		{
			NowSupplies = true;
			_task = new Task(() =>
			{
				while (NowSupplies)
				{
					Storages.ForEach(storage => storage.Add(new Product(Product)));
					Thread.Sleep(Interval);
				}
			});
			_task.Start();
		}

		public void FinishDeliveries() => NowSupplies = false;
	}

	class Consumer
	{
		Task _task;

		public Consumer(int interval, Storage storage)
		{
			Interval = interval;
			Storage = storage;
		}

		public int Interval { get; set; }
		public Storage Storage { get; private set; }
		public bool NowBuying { get; private set; }

		public void StartBuying()
		{
			NowBuying = true;
			_task = new Task(() =>
			{
				Thread.Sleep(5000);
				while (NowBuying)
				{
					Storage.Take();
					Thread.Sleep(Interval);
				}
			});
			_task.Start();
		}

		public void FinishBuying() => NowBuying = false;
	}
}