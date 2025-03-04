using Microsoft.AspNetCore.Mvc;
using TimeTableGenerator.Models;

namespace TimeTableGenerator.Controllers
{
	public class TimeTableController : Controller
	{
		public IActionResult Index()
		{
			return View(new TimeTableInputModel());
		}

		[HttpPost]
		public IActionResult SetSubjects(TimeTableInputModel model)
		{
			if (!ModelState.IsValid)
				return View("Index", model);

			int totalHours = model.TotalHours;
			ViewBag.TotalHours = totalHours;
			ViewBag.SubjectsCount = model.TotalSubjects;

			return View("EnterSubjects");
		}

		[HttpPost]
		public IActionResult GenerateTimeTable(List<SubjectHoursModel> subjects)
		{
			int totalEnteredHours = subjects.Sum(s => s.Hours);
			int expectedHours = int.Parse(Request.Form["TotalHours"]);

			if (totalEnteredHours != expectedHours)
			{
				ViewBag.Error = "Total subject hours must match total working hours!";
				ViewBag.TotalHours = expectedHours; // Ensure ViewBag.TotalHours is set
				ViewBag.SubjectsCount = subjects.Count; // Ensure ViewBag.SubjectsCount is set

				return View("EnterSubjects", subjects);
			}

			var timetable = new List<List<string>>();
			var subjectList = new List<string>();

			//Total subjects
			foreach (var subject in subjects)
			{
				subjectList.AddRange(Enumerable.Repeat(subject.SubjectName, subject.Hours));
			}

			//Shuffle subjects
			var rnd = new Random();
			subjectList = subjectList.OrderBy(x => rnd.Next()).ToList();

			//Generating dynamic table
			for (int i = 1; i < expectedHours / subjects.Count; i++)
			{
				//Check if there are enough subjects left to fill one day.
				if (subjectList.Count >= subjects.Count)
				{
					//Take a chunk of subjects.Count items for this day.
					var daySchedule = subjectList.Take(expectedHours / subjects.Count).ToList();
					timetable.Add(daySchedule);
					//Remove the used items.
					subjectList.RemoveRange(0, expectedHours / subjects.Count);
				}
				else if (subjectList.Count > 0)
				{
					//If there are any remaining items (this should be rare if the math works out),
					//add them as the final day.
					timetable.Add(new List<string>(subjectList));
					subjectList.Clear();
				}
			}

			return View("GeneratedTimeTable", timetable);
		}

	}
}
