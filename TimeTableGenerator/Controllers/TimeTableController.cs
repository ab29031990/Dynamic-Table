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

			foreach (var subject in subjects)
				subjectList.AddRange(Enumerable.Repeat(subject.SubjectName, subject.Hours));

			var rnd = new Random();
			for (int i = 0; i < expectedHours / subjects.Count; i++)
			{
				timetable.Add(subjectList.OrderBy(_ => rnd.Next()).Take(subjects.Count).ToList());
			}

			return View("GeneratedTimeTable", timetable);
		}

	}
}
