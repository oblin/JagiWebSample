/**
 * 提供所有 app.js 使用，主要提供以下兩個 service:
 * 1. dataService: 作為 http.get & http.post 的服務，包含處理錯誤訊息等，並搭配 _ValidationErrors.cshtml 顯示
 * 2. validator: 提供前端一次性的判斷所有輸入欄位是否有錯誤（主要用在 save 時候）
 */
(function () {
    common = angular.module('common', []);

    common.factory('dataService', function ($http, $q) {
        var self = this;

        self.isValid = true;
        self.errors = [];
        self.isLoading = false;

        self.get = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.isValid = true;
            $http.get(uri, data)
                .then(function (result) {
                    success(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                }, function (result) {
                    if (failure == null) {
                        if (result.status != 400)
                            self.errors = [result.status + ':' + result.statusText + ' - ' + result.data.Message];
                        else
                            self.errors = [result.data.Message];
                        self.isValid = false;
                    }
                    else
                        failure(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                });
        }

        self.post = function (uri, data, success, failure, always) {
            self.isLoading = true;
            self.isValid = true;
            $http.post(uri, data)
                .then(function (result) {
                    success(result);
                    self.isValid = true;
                    self.isLoading = false;
                    if (always != null)
                        always();
                }, function (result) {
                    if (failure == null) {
                        if (result.status != 406)
                            self.errors = [result.status + ':' + result.statusText + ' - ' + result.data.Message];
                        else
                            self.errors = result.data.errorMessages;
                        self.isValid = false;
                    }
                    else
                        failure(result);

                    if (always != null)
                        always();
                    isLoading = false;
                });
        }

        return this;
    })

    common.factory('validator', function () {
        var self = this;

        self.propertyRule = function (propertyName, rules) {
            var self = this;
            self.propertyName = propertyName;
            self.rules = rules;
        };

        self.ValidateModel = function (model, allPropertyrules) {
            var errors = [];
            var props = Object.keys(model);
            for (var i = 0; i < allPropertyrules.length; i++) {
                // 檢查必要欄位，但 model 並未包含
                var propRule = allPropertyrules[i];
                if (propRule.rules["required"])
                    if (props.indexOf(propRule.propertyName) < 0)
                        errors.push(getMessage(propRule.propertyName, "required", propRule.rules["required"].message));
            }

            for (var i = 0; i < props.length; i++) {
                var prop = props[i];
                for (var j = 0; j < allPropertyrules.length; j++) {
                    var propertyRule = allPropertyrules[j];
                    if (prop == propertyRule.propertyName) {
                        var propertyrules = propertyRule.rules;

                        var propertyRuleProps = Object.keys(propertyrules);
                        for (var k = 0; k < propertyRuleProps.length; k++) {
                            var propertyRuleProp = propertyRuleProps[k];
                            if (propertyRuleProp != 'custom') {
                                var rule = rules[propertyRuleProp];
                                var params = null;
                                if (propertyrules[propertyRuleProp].hasOwnProperty('parameters'))
                                    params = propertyrules[propertyRuleProp].parameters;
                                var validationResult = rule.validator(model[prop], params);
                                if (!validationResult) {
                                    var message = getMessage(prop, propertyRule.rules[propertyRuleProp], rule.message);
                                    errors.push(getMessage(prop, propertyRuleProp, message));
                                }
                            }
                            else {
                                var validator = propertyrules.custom.validator;
                                var value = null;
                                if (propertyrules.custom.hasOwnProperty('parameters')) {
                                    value = propertyrules.custom.parameters;
                                }
                                var result = validator(model[prop], value());
                                if (result != true) {
                                    errors.push(getMessage(prop, propertyrules.custom, 'Invalid value.'));
                                }
                            }
                        }
                    }
                }
            }

            model['errors'] = errors;
            model['isValid'] = (errors.length == 0);
        }

        var getMessage = function (prop, rule, defaultMessage) {
            var message = '';
            if (rule.hasOwnProperty('message'))
                message = rule.message;
            else
                message = prop + ': ' + defaultMessage;
            return message;
        }

        var rules = [];

        var setupRules = function () {

            rules['required'] = {
                validator: function (value, params) {
                    if (angular.isNumber(value))
                        return true;
                    return value && !(value.trim() == '');
                },
                message: 'Value is required.'
            };
            rules['minLength'] = {
                validator: function (value, params) {
                    return !(value.trim().length < params);
                },
                message: 'Value does not meet minimum length.'
            };
            rules['maxLength'] = {
                validator: function (value, params) {
                    return !value || !(value.trim().length > params);
                },
                message: '輸入字串超過最大位數'
            };
            rules['pattern'] = {
                validator: function (value, params) {
                    var regExp = new RegExp(params);
                    return !(regExp.exec(value.trim()) == null);
                },
                message: 'Value must match regular expression.'
            };
        }

        setupRules();

        return this;
    })

})();