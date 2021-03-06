﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermaFx.DataAnnotations
{
	public class AggregateValidationException : ValidationException
	{
		private const string DefaultErrorMessage = "Object of type {0} has some invalid values.";

		public IEnumerable<ValidationResult> ValidationResults { get; private set; }
		public IEnumerable<ValidationException> ValidationExceptions { get; private set; }

		public AggregateValidationException(string errorMessage, IEnumerable<ValidationException> exceptions)
			: base(AggregateResultsFrom(errorMessage, exceptions), null, null)
		{
			ValidationResults = (base.ValidationResult as AggregateValidationResult).Results;
			ValidationExceptions = exceptions.ToArray();
		}

		public AggregateValidationException(string errorMessage, AggregateValidationResult validationResult)
			: base(validationResult, null, null)
		{
			ValidationResults = validationResult.Results;
			ValidationExceptions = validationResult.Flatten().Select(x => new ValidationException(x.ErrorMessage));
		}

		#region Helper Methods
		private static AggregateValidationResult AggregateResultsFrom(string message, IEnumerable<ValidationException> exceptions)
		{
			Guard.IsNotNull(exceptions, "exceptions");

			return AggregateValidationResult.CreateFor(message, exceptions.Select(x => x.ValidationResult));
		}
		#endregion

		#region Factory Ctors
		public static AggregateValidationException CreateFor<T>(T obj, IEnumerable<ValidationException> exceptions = null)
		{
			return CreateFor(typeof(T), exceptions);
		}

		public static AggregateValidationException CreateFor(Type type, IEnumerable<ValidationException> exceptions = null)
		{
			return new AggregateValidationException(
				string.Format(DefaultErrorMessage, type.Name),
				exceptions
			);
		}

		public static AggregateValidationException CreateFor<T>(T obj, IEnumerable<ValidationResult> results = null)
		{
			return CreateFor(typeof(T), results);
		}

		public static AggregateValidationException CreateFor(Type type, IEnumerable<ValidationResult> results = null)
		{
			return new AggregateValidationException(
				string.Format(DefaultErrorMessage, type.Name),
				AggregateValidationResult.CreateFor(type, results)
			);
		}
		#endregion

	}
}
