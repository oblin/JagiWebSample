(function() {
	'use strict';

	Object.defineProperty(Array.prototype, 'count', {
		get: function() { return this.length; }
	});

	if (Array.prototype.addRange) return;

	Array.prototype.addRange = function (target) {
		this.push.apply(this, target);
	};

	if (Array.prototype.getIndexBy) return;

	Array.prototype.getIndexBy = function (name, value) {
	    for (var i = 0; i < this.length; i++) {
	        if (this[i][name] == value) {
	            return i;
	        }
	    }
	}

	if (Array.prototype.getIndexById) return;

	Array.prototype.getIndexById = function (value) {
        return this.getIndexBy("id", value);
	}
})();