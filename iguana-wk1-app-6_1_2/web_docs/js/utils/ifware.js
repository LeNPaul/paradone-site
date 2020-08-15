/** @license
 * Copyright (c) 2010-2013 iNTERFACEWARE Inc.  All rights reserved.
 */

ifware = window.ifware || {};

(function(ifware) {
    $lambda =  function(x) { return function() { return x; }; };  // intentional global
    $empty = function() { };  // intentional global
    $inherit = function(sub, base, methods) {
        var surrogate = $empty; // used to set up prototype chain for inheriting
        surrogate.prototype = base.prototype;
        sub.prototype = new surrogate();
        sub.prototype.constructor = sub;
        // Add a reference to the parent's prototype
        sub.base = base;

        // Copy the methods passed in to the prototype
        for (var name in methods) {
            sub.prototype[name] = methods[name];
        }
        // so we can define the constructor inline
        return sub;
    };

    /** Create a state object and attach it to $this.data,
     * and also support inheritance by merging states with the current state.
     * @param $this
     * @param defaults
     * @param userOptions
     * @param parentClass
     * @return {state}
     */
    ifware.extendFrom = function init($this, defaults, userOptions, parentClass){
        if (parentClass) parentClass($this, userOptions);
        $.extend(defaults, userOptions);
        if ($this.data('state')) {
            $.extend($this.data('state'), defaults);
        } else {
            $this.data('state', defaults);
        }
        return $this.data('state');
    };
})(ifware);

