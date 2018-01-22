using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Base class for view model dtos to validate them
    /// </summary>
    public class ValidationViewModelBase : PropertyChangedBase, IDataErrorInfo
    {
        private readonly Dictionary<string, IValidationRule> _valiedationProperties = new Dictionary<string, IValidationRule>();
        private readonly ValidationType _validationType;

        #region Properties

        /// <summary>
        /// Will check the view model properties for validation errors
        /// </summary>
        public bool IsValid
        {
            get { return _valiedationProperties.All(property => string.IsNullOrEmpty(GetValidationError(property.Key))); }
        }

        /// <summary>
        /// Will check the view model properties for validation errors and will return all errors
        /// </summary>
        public IEnumerable<string> ValidationErrors
        {
            get { return _valiedationProperties.Select(p => GetValidationError(p.Key)).ToArray(); }
        }

        /// <summary>
        /// Implementation of the <see cref="IDataErrorInfo"/> for wpf validation
        /// </summary>
        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationViewModelBase"/> class.
        /// </summary>
        /// <param name="validationType">Type of the validation.</param>
        protected ValidationViewModelBase(ValidationType validationType)
        {
            _validationType = validationType;

            if (_validationType == ValidationType.DataAnnotation)
            {
                var validationProps = GetType().GetProperties().Where(p => p.GetCustomAttributes<ValidationAttribute>().Any());
                foreach (var property in validationProps)
                    _valiedationProperties[property.Name] = NullValidator.Instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationViewModelBase"/> class.
        /// </summary>
        protected ValidationViewModelBase() : this(ValidationType.DataAnnotation)
        {
            
        }

        /// <summary>
        /// Implementation of the <see cref="IDataErrorInfo"/> for wpf validation
        /// </summary>
        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// Adds a new validation rule.
        /// </summary>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="expression">The <see cref="MemberExpression"/> for the property</param>
        /// <param name="rule">An implementation of <see cref="IValidationRule"/> to validate the property</param>
        protected void AddValidationRule<TProp>(Expression<Func<TProp>> expression, IValidationRule rule)
        {
            if (_validationType != ValidationType.Rules)
                throw new InvalidOperationException("Cannot add validation rule to view model validation type: " + _validationType);

            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("Should be a member expression", "expression");

            _valiedationProperties[body.Member.Name] = rule;
        }

        /// <summary>
        /// Gets the validation error by property name.
        /// </summary>
        private string GetValidationError(string propertyName)
        {
            if (_validationType == ValidationType.Rules)
                return GetValidationErrorByRule(propertyName);

            if (_validationType == ValidationType.DataAnnotation)
                return GetValidationErrorByAnnotation(propertyName);

            return string.Empty;
        }

        /// <summary>
        /// Will validate the property by defined rules added by <see cref="AddValidationRule{TProp}"/>
        /// </summary>
        private string GetValidationErrorByRule(string property)
        {
            if (!_valiedationProperties.ContainsKey(property))
                return "Validation property not found";

            var validator = _valiedationProperties[property];
            var value = GetPropValue(property);

            var valid = validator.Validate(value);

            return !valid ? validator.ErrorMessage : string.Empty;
        }

        /// <summary>
        /// Will validate the property be the defined annotations 
        /// attributes derived by <see cref="ValidationAttribute"/>
        /// </summary>
        private string GetValidationErrorByAnnotation(string property)
        {
            string error = string.Empty;
            var value = GetPropValue(property);
            var results = new List<ValidationResult>(1);
            var result = Validator.TryValidateProperty(
                value, new ValidationContext(this, null, null)
                {
                    MemberName = property
                }, results);

            if (!result)
            {
                var validationResult = results.First();
                error = validationResult.ErrorMessage;
            }

            return error;
        }

        /// <summary>
        /// Gets the value of the given property be reflection
        /// </summary>
        private object GetPropValue(string propName)
        {
            return GetType().GetProperty(propName).GetValue(this, null);
        }
    }
}