using System;
using System.Collections;
using System.Collections.Generic;

namespace GameData
{
	[Serializable]
	public class Data
	{
		public int RunCount = 0;
		public RunData[] runs = new RunData[10];

		public int GetPlace(RunData run) {
			for (int i = 0; i < runs.Length; i++) {
				if (RunData.CompareRuns(runs[i], run) == 0) {
					return i+1;
				}
			}
			return -1;
		}

		public void AddToRuns(RunData newRun) {
			RunCount++;
			RunData temp = newRun;
			int index = -1;
			for (int i = 0; i < runs.Length; i++) {
				if (runs[i] == null) {
					index = i;
					temp = null;
					break;
				}

				if (newRun.Compare(temp, runs[i]) < 0) {
					index = i;
					temp = runs[i];
				}
			}

			if (temp != newRun) {
				runs[index] = newRun;
				newRun.runNumber = RunCount;
				Array.Sort(runs, RunData.CompareRuns);
			}
		}
	}

	[Serializable]
	public class RunData : IComparer<RunData> {
		public int runNumber = 0;
		public int score;
		public int distance;

		public RunData(int score, int distance) {
			this.score = score;
			this.distance = distance;
		}

		public int Compare(RunData x, RunData y) {
			return CompareRuns(x, y);
		}

		public static int CompareRuns(RunData x, RunData y) {
			if (x == y) { return 0; }
			if (x == null) {
				return 1;
			}
			if (y == null) {
				return -1;
			}

			if (x.score > y.score) {
				return -1;
			}
			else if (x.score < y.score) {
				return 1;
			}
			return 0;
		}
	}

	/*
		В этом методе сравниваются объекты х и у возвращается 
		нулевое значение, если значения сравниваемых объектов равны; 
		положительное — если значение объекта х больше, чем у объекта у;
		отрицательное — если значение объекта х меньше, чем у объекта у.
	*/

}
