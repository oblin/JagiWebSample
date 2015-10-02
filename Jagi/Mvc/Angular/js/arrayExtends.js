(function () {
    'use strict';

    /**
     * 擴充 Array.count 定義，傳回 Array.length 主要目的： C# IList 有 Count property 對應
     */
    Object.defineProperty(Array.prototype, 'count', {
        get: function () { return this.length; }
    });

    if (Array.prototype.addRange) return;

    /**
     * 將陣列加入到另外一個陣列內
     * @param target
     */
    Array.prototype.addRange = function (target) {
        this.push.apply(this, target);
    };
})();