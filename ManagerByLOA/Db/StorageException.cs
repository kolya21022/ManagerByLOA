using System;
using System.Data.Common;

namespace ManagerByLetterOfAttorney.Db
{
	/// <inheritdoc />
	/// <summary>
	/// Исключение слоя работы с базой данных.
	/// В некоторых случаях указывается возможная `человекочитаемая` причина возникновения, 
	/// для информирования конечных пользователей, в специальном окне приложения.
	/// </summary>
	public class StorageException : DbException
	{
		private readonly string _probableCause; // Возможная причина

		/// <summary>
		/// Возможная причина
		/// </summary>
		public string ProbableCause
		{
			get { return _probableCause; }
		}

		public StorageException(string message) : base(message) { }

		public StorageException(string message, string probableCause, Exception innerException)
			: base(message, innerException)
		{
			_probableCause = probableCause;
		}
		
		public StorageException(string message, Exception innerException) : base(message, innerException) { }
	}
}
