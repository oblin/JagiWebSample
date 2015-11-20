(function(){
    'use strict';

    var dateFormat = "YYYY/MM/DD";

    String.convertToDate = function () {
        var s = arguments[0];
        return convertToDate(s);
    }

    String.prototype.convertToDate = function () {
        var s = this.toString();
        return convertToDate(s);
    }

    function convertToDate(s) {
        if (s == null) return "";

        if (s.length == 0 || s.length < 6) return "";

        if (s.length == 6) {
            var result = dealSlashDate(s);
            if (result.length == 10)
                return result;
            // 處理 990101
            s = '0' + s;
        }
        if (s.length == 7) {
            var result = dealSlashDate(s);
            if (result.length == 10)
                return result;
            // 處理 0990101 or 1031201
            var y = parseInt(s.substr(0, 3), 10) + 1911;
            var m = parseInt(s.substr(3, 2), 10);
            var d = parseInt(s.substr(5, 2), 10);
            var date = y + "/" + m + "/" + d;
            //if (moment(date).isValid()) {
            //    var returnDate = moment(date).format(dateFormat);
            //    var stringDate = returnDate.toString();
            //    return stringDate;
            //}
            //else
            //    return "";
            return getDateString(date);
        }
        if (s.length == 8 || s.length == 9) {
            var result = dealSlashDate(s);
            if (result.length == 10)
                return result;
        }

        return getDateString(s);
        //if (moment(s).isValid()) {
        //    var returnDate = moment(s).format(dateFormat);
        //    var stringDate = returnDate.toString();
        //    return stringDate;
        //} else {
        //    return "";
        //}
    }

    function dealSlashDate(s) {
        if (s.indexOf('/') > -1) {
            var array = s.split('/');
            if (array.length == 3) {
                var y = parseInt(array[0]) + 1911;
                var m = parseInt(array[1], 10);
                var d = parseInt(array[2], 10);
                var date = y + "/" + m + "/" + d;
                return getDateString(date);
            }
        }
        return s;
    }

    function getDateString(s) {
        if (moment(s).isValid()) {
            var returnDate = moment(s).format(dateFormat);
            var stringDate = returnDate.toString();
            return stringDate;
        } else {
            return "";
        }    }

    /**
     * 可在Javascript中使用如同C#中的string.format
     * 使用方式 : var fullName = String.format('Hello. My name is {0} {1}.', 'FirstName', 'LastName');
     * @returns {type} 
     */
    String.format = function () {
        var s = arguments[0];
        if (s == null) return "";

        for (var i = 0; i < arguments.length - 1; i++) {
            var reg = getStringFormatPlaceHolderRegEx(i);
            s = s.replace(reg, (arguments[i + 1] == null ? "" : arguments[i + 1]));
        }

        return cleanStringFormatResult(s);
    }

    /**
     * 可在Javascript中使用如同C#中的string.format (對jQuery String的擴充方法)
     * 使用方式 : var fullName = 'Hello. My name is {0} {1}.'.format('FirstName', 'LastName');
     * @returns {type} 
     */
    String.prototype.format = function () {
        var txt = this.toString();
        for (var i = 0; i < arguments.length; i++) {
            var exp = getStringFormatPlaceHolderRegEx(i);
            txt = txt.replace(exp, (arguments[i] == null ? "" : arguments[i]));
        }

        return cleanStringFormatResult(txt);
    }

    /**
     * 讓輸入的字串可以包含{}
     * @param {type} placeHolderIndex
     * @returns {type} 
     */
    function getStringFormatPlaceHolderRegEx(placeHolderIndex) {
        return new RegExp('({)?\\{' + placeHolderIndex + '\\}(?!})', 'gm')
    }

    /**
     * 當format格式有多餘的position時，就不會將多餘的position輸出
     * var fullName = 'Hello. My name is {0} {1} {2}.'.format('firstName', 'lastName');
     * 輸出的 fullName 為 'firstName lastName', 而不會是 'firstName lastName {2}'
     * @param {type} txt
     * @returns {type} 
     */
    function cleanStringFormatResult(txt) {
        if (txt == null) return "";

        return txt.replace(getStringFormatPlaceHolderRegEx("\\d+"), "");
    }
})();