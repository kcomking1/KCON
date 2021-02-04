/*!
 * Distpicker v1.0.2
 * https://github.com/tshi0912/city-picker
 *
 * Copyright (c) 2014-2016 Tao Shi
 * Released under the MIT license
 *
 * Date: 2016-02-29T12:11:36.473Z
 */

(function (factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as anonymous module.
        define('ChineseDistricts', [], factory);
    } else {
        // Browser globals.
        factory();
    }
})(function () {
    var ChineseJson = $.ajax({ url: "/Scripts/CityPicker/cityPicker.json", async: false }).responseText;
    var ChineseJsonVal = $.parseJSON(ChineseJson);
    var ChineseDistricts = ChineseJsonVal.ChineseDistricts;
    if (typeof window !== 'undefined') {
        window.ChineseDistricts = ChineseDistricts;
    }
    return ChineseDistricts;

});
