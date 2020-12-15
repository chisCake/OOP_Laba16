using System.Collections.Generic;
using System.Threading;

namespace OOP_Laba16 {
	static class PrimeNumbers {
		public static void Eratosfen(object data) {
			int max = (int)data;
			var primes = new List<int>();
			for (int i = 1; i < max; i++)
				primes.Add(i);
			DoEratosfen();

			int Step(int num, int startFrom) {
				int i = startFrom + 1;
				int Removed = 0;
				while (i < primes.Count)
					if (primes[i] % num == 0) {
						primes.RemoveAt(i);
						Removed++;
					}
					else
						i++;
				return Removed;
			}

			void DoEratosfen() {
				int i = 1;
				while (i < primes.Count) {
					Step(primes[i], i);
					i++;
				}
			}
		}

		public static void Simple(object data) {
			int max = (int)data;
			var primes = new List<int>();
			for (int i = 1; i < max; i++)
				if (IsPrime(i))
					primes.Add(i);

			static bool IsPrime(int n) {
				for (var i = 2; i < n; i++)
					if (n % i == 0)
						return false;
				return true;
			}
		}

		public static void Eratosfen(object data, CancellationToken token) {
			int max = (int)data;
			var primes = new List<int>();
			for (int i = 1; i < max; i++)
				primes.Add(i);
			DoEratosfen();

			int Step(int num, int startFrom) {
				int i = startFrom + 1;
				int Removed = 0;
				while (i < primes.Count) {
					// Отмена тут
					if (token.IsCancellationRequested)
						break;
					if (primes[i] % num == 0) {
						primes.RemoveAt(i);
						Removed++;
					}
					else
						i++;
				}
				return Removed;
			}

			void DoEratosfen() {
				int i = 1;
				while (i < primes.Count) {
					// И тут
					if (token.IsCancellationRequested)
						break;
					Step(primes[i], i);
					i++;
				}
			}
		}
	}
}