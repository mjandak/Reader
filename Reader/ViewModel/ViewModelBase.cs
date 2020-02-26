using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace Reader
{
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public virtual bool IsValid { get; } = true;

        protected virtual void PropChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void PropToValidateChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
        }

        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
        
        private object _lock = new object();

        public bool HasErrors => _errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (_errors.ContainsKey(propertyName) && (_errors[propertyName] != null) && _errors[propertyName].Count > 0)
                    return _errors[propertyName].ToList();
                else
                    return null;
            }
            else
                return _errors.SelectMany(err => err.Value.ToList());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this) { MemberName = propertyName };
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                //clear previous _errors from tested property  
                if (_errors.ContainsKey(propertyName)) _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
                HandleValidationResults(validationResults);
            }
        }

        /// <summary>
        /// Validates all properties on model and updates list of errors. <see cref="HasErrors"/> get changed.
        /// </summary>
        /// <param name="notify"></param>
        /// <returns><see cref="HasErrors"/></returns>
        public bool Validate(bool notify = true)
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //clear all previous _errors  
                //var propNames = _errors.Keys.ToList();
                //_errors.Clear();
                //if (notify) propNames.ForEach(pn => OnErrorsChanged(pn));

                //clear all previous _errors
                foreach (string propName in _errors.Keys.ToArray())
                {
                    _errors.Remove(propName);
                    if (notify) OnErrorsChanged(propName);
                }

                HandleValidationResults(validationResults, notify);
                return HasErrors;
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults, bool notify = true)
        {
            //Group validation results by property names  
            IEnumerable<IGrouping<string, ValidationResult>> resultsByPropNames = 
                from res in validationResults
                from mname in res.MemberNames
                group res by mname into g
                select g;
            
            //add _errors to dictionary and inform binding engine about _errors  
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                _errors.Add(prop.Key, messages);
                if (notify) OnErrorsChanged(prop.Key);
            }
            PropChanged(nameof(HasErrors));
        }
    }

    //interface IValidatable : INotifyPropertyChanged
    //{
    //    bool IsValid { get; }

    //    void PropToValidateChanged(string name);
    //}
}
