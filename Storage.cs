using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OOP_Laba16 {
	class Storage {
		public BlockingCollection<Product> Products { get; private set; }

		public Storage() {
			Products = new BlockingCollection<Product>();
		}

		public void Add(Product product) {
			Products.Add(product);
			Console.WriteLine($"+ {product.Name}\n");
			PrintProducts();
		}

		public Product Take() {
			var product = Products.Take();
			if (product != null) {
				Console.WriteLine($"- {product.Name}\n");
				PrintProducts();
			}
			return product;
		}

		public void PrintProducts() {
			Console.Clear();
			foreach (var item in Products.OrderBy(item => item.ID))
				Console.WriteLine($"{item.Name, -20} {item.ID}");
		}
	}

	class Product {
		public string Name { get; }
		public int ID { get; }

		public Product(string name) {
			Name = name;
			ID = rdnm.Next(100000, 1000000);
		}

		public override string ToString() => $"{Name} {ID}";

		static readonly Random rdnm = new Random();
	}

	class Supplier {
		public string Product { get; set; }
		public int Interval { get; set; }
		public List<Storage> Storages { get; private set; }
		public bool NowSupplies { get; private set; }
		Task task;

		public Supplier(string product, int interval, params Storage[] storages) {
			Product = product;
			Interval = interval;
			Storages = new List<Storage>(storages);
		}

		public void AddStorage(Storage storage) => Storages.Add(storage);

		public void RemoveStorage(Storage storage) => Storages.Remove(storage);

		public void StartDeliveries() {
			NowSupplies = true;
			task = new Task(() => {
				while (NowSupplies) {
					Storages.ForEach(storage => storage.Add(new Product(Product)));
					Thread.Sleep(Interval);
				}
			});
			task.Start();
		}

		public void FinishDeliveries() {
			NowSupplies = false;
		}
	}

	class Consumer {
		public int Interval { get; set; }
		public Storage Storage { get; private set; }
		public bool NowBuying { get; private set; }
		Task task;

		public Consumer(int interval, Storage storage) {
			Interval = interval;
			Storage = storage;
		}

		public void StartBuying() {
			NowBuying = true;
			task = new Task(() => {
				Thread.Sleep(5000);
				while (NowBuying) {
					Storage.Take();
					Thread.Sleep(Interval);
				}
			});
			task.Start();
		}

		public void FinishBuying() {
			NowBuying = false;
		}
	}
}
