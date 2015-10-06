using HtmlTags;
using Humanizer;
using Jagi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Jagi.Mvc.Angular
{
    public class AngularModelHelper<TModel>
    {
        protected readonly HtmlHelper _helper;
        private readonly string _expressionPrefix;

        public AngularModelHelper(HtmlHelper helper, string expressionPrefix)
        {
            _helper = helper;
            _expressionPrefix = expressionPrefix;
        }

        /// <summary>
        /// Converts an lambda expression into a camel-cased string, prefixed
        /// with the helper's configured prefix expression, ie:
        /// vm.model.parentProperty.childProperty
        /// </summary>
        public IHtmlString ExpressionFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            var expressionText = property.ExpressionForInternal(_expressionPrefix);
            return new MvcHtmlString(expressionText);
        }

        /// <summary>
        /// Converts a lambda expression into a camel-cased AngularJS binding expression, ie:
        /// {{vm.model.parentProperty.childProperty}}
        /// </summary>
        public IHtmlString BindingFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            return MvcHtmlString.Create("{{" + property.ExpressionForInternal(_expressionPrefix) + "}}");
        }

        /// <summary>
        /// Creates a div with an ng-repeat directive to enumerate the specified property,
        /// and returns a new helper you can use for strongly-typed bindings on the items
        /// in the enumerable property.
        /// 使用方式：
        ///     @using(var object = model.Repeat(x => x.objects, "object")) {
        ///         <li>@object.BindingFor(x => x.Title)</li>
        ///         <li>@object.BindingFor(x => x.Name)</li>
        ///     }
        /// </summary>
        public AngularNgRepeatHelper<TSubModel> Repeat<TSubModel>(
            Expression<Func<TModel, IEnumerable<TSubModel>>> property, string variableName)
        {
            var propertyExpression = property.ExpressionForInternal();
            return new AngularNgRepeatHelper<TSubModel>(
                _helper, variableName, propertyExpression);
        }

        public HtmlTag AngularLabelFor<TProp>(Expression<Func<TModel, TProp>> property)
        {
            AngularHtmlTag ngControl = AngularHtmlTagFactory.Get(property, _expressionPrefix);
            return ngControl.GetLabel();
        }

        public HtmlTag AngularEditorFor<TProp>(Expression<Func<TModel, TProp>> property,
            FormGroupType type = FormGroupType.Default,
            string attr = null,
            Dictionary<string, string> attrs = null,
            Dictionary<string, string> options = null)
        {
            AngularHtmlTag ngControl = AngularHtmlTagFactory.Get(property, _expressionPrefix);
            HtmlTag input = ngControl.GetInput(type, options);

            ngControl.ApplyValidationToInput(input);

            ngControl.ApplyCustomizedAttributes(input, attr, attrs);

            return input;
        }

        public HtmlTag FormGroupFor<TProp>(Expression<Func<TModel, TProp>> property, 
            FormGroupType type = FormGroupType.Default,
            string attr = null,
            Dictionary<string, string> attrs = null,
            Dictionary<string, string> options = null)
        {
            AngularHtmlTag ngControl = AngularHtmlTagFactory.Get(property, _expressionPrefix);

            string name = ngControl.GetName();

            HtmlTag formGroup = new HtmlTag("div")
                .AddClasses("form-group", "has-feedback")
                .Attr("form-group-validation", name);

            HtmlTag label = AngularLabelFor(property);

            HtmlTag input = AngularEditorFor(property, type, attr, attrs, options);

            //HtmlTag input = ngControl.GetInput(type, options);

            //ngControl.ApplyValidationToInput(input);

            //ngControl.ApplyCustomizedAttributes(input, attr, attrs);

            //var angularMapper = new AngularModelDecorator(metadata);

            //var name = ExpressionHelper.GetExpressionText(property);
            //var labelText = metadata.DisplayName ?? name.Humanize(LetterCasing.Title);

            //var expression = ExpressionForInternal(property);

            ////Creates <div class="form-group has-feedback"
            ////				form-group-validation="Name">
            //var formGroup = new HtmlTag("div")
            //    .AddClasses("form-group", "has-feedback")
            //    .Attr("form-group-validation", name);

            ////Creates <label class="control-label" for="Name">Name</label>
            //var label = new HtmlTag("label")
            //    .AddClass("control-label")
            //    .Attr("for", name)
            //    .Text(labelText);

            //HtmlTag input = angularMapper.CreateInput(type, name, options);

            //input.AddClass("form-control")
            //     .Attr("name", name)
            //     .Attr("ng-model", expression);

            //SetupCustomizedAttributes(attr, attrs, input);

            //ApplyValidationToInput(input, metadata);

            return formGroup
                .Append(label)
                .Append(input);
        }
    }
}