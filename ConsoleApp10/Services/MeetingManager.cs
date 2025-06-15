using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MeetingApp.Models;

namespace MeetingApp.Services
{
    /// <summary>
    /// Сервис для управления встречами: добавление, обновление, удаление и экспорт.
    /// </summary>
    public class MeetingManager
    {
        private List<Meeting> _meetings = new();

        /// <summary>
        /// Получает встречи на конкретную дату.
        /// </summary>
        /// <param name="date">Дата, для которой нужно получить встречи.</param>
        /// <returns>Список встреч на указанную дату.</returns>
        public IEnumerable<Meeting> GetMeetingsForDay(DateTime date) =>
            _meetings.Where(m => m.Start.Date == date.Date).OrderBy(m => m.Start);

        /// <summary>
        /// Добавляет новую встречу, если она не пересекается с другими и запланирована на будущее.
        /// </summary>
        /// <param name="newMeeting">Встреча, которую нужно добавить.</param>
        /// <exception cref="Exception">Возникает, если встреча запланирована некорректно или пересекается с другой встречей.</exception>
        public void AddMeeting(Meeting newMeeting)
        {
            if (newMeeting.Start <= DateTime.Now || newMeeting.End <= newMeeting.Start)
                throw new Exception("Встреча запланирована некорректно.");

            if (_meetings.Any(m => m.Start < newMeeting.End && m.End > newMeeting.Start))
                throw new Exception("Перекрытие с другой встречей.");

            _meetings.Add(newMeeting);
        }

        /// <summary>
        /// Обновляет встречу с указанным ID.
        /// </summary>
        /// <param name="id">ID встречи, которую нужно обновить.</param>
        /// <param name="newTitle">Новое название встречи.</param>
        /// <param name="newStart">Новое начало встречи.</param>
        /// <param name="newEnd">Новое окончание встречи.</param>
        /// <param name="newReminderOffset">Новое напоминание о встрече.</param>
        /// <exception cref="Exception">Возникает, если встреча не найдена или не удаётся обновить встречу.</exception>
        public void UpdateMeeting(Guid id, string newTitle, DateTime newStart, DateTime newEnd, TimeSpan? newReminderOffset)
        {
            var meeting = _meetings.FirstOrDefault(m => m.Id == id);
            if (meeting == null)
                throw new Exception("Встреча не найдена.");

            // Сохраняем старые значения на случай, если новая встреча не пройдёт проверку
            var oldTitle = meeting.Title;
            var oldStart = meeting.Start;
            var oldEnd = meeting.End;
            var oldReminderOffset = meeting.ReminderOffset;

            meeting.Title = newTitle;
            meeting.Start = newStart;
            meeting.End = newEnd;
            meeting.ReminderOffset = newReminderOffset;

            // Проверяем, что обновлённая встреча не пересекается с другими и запланирована на будущее
            try
            {
                AddMeeting(meeting);
            }
            catch (Exception e)
            {
                // Если проверка не прошла, возвращаем исходные значения
                meeting.Title = oldTitle;
                meeting.Start = oldStart;
                meeting.End = oldEnd;
                meeting.ReminderOffset = oldReminderOffset;
                throw;
            }

        }

        /// <summary>
        /// Удаляет встречу по ID.
        /// </summary>
        /// <param name="id">ID встречи, которую нужно удалить.</param>
        /// <exception cref="Exception">Возникает, если встреча не найдена.</exception>
        public void DeleteMeeting(Guid id)
        {
            if (_meetings.RemoveAll(m => m.Id == id) == 0)
                throw new Exception("Встреча не найдена.");
        }

        /// <summary>
        /// Экспортирует встречи на день в указанный файл.
        /// </summary>
        /// <param name="date">Дата, встречи на которую нужно экспортировать.</param>
        /// <param name="path">Путь к файлу для экспорта.</param>
        public void ExportDaySchedule(DateTime date, string path)
        {
            var lines = GetMeetingsForDay(date).Select(m => m.ToString());
            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// Получает встречи, по которым нужно отправить напоминание.
        /// </summary>
        /// <returns>Список встреч, по которым нужно отправить напоминание.</returns>
        public List<Meeting> GetUpcomingReminders()
        {
            var now = DateTime.Now;

            return _meetings
                .Where(m => m.ReminderTime.HasValue && m.ReminderTime.Value <= now && m.Start > now)
                .ToList();
        }

        /// <summary>
        /// Удаляет напоминание у встречи.
        /// </summary>
        /// <param name="id">ID встречи, у которой нужно удалить напоминание.</param>
        public void RemoveReminder(Guid id)
        {
            var meeting = _meetings.FirstOrDefault(m => m.Id == id);
            if (meeting != null)
                meeting.ReminderOffset = null;
        }

    }

}
