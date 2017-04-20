// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

var Formatters = (function () {

    var formatLabel = function (result, element, singular, plural) {
        if (result == 1) {
            element[0].textContent = singular;
        } else {
            element[0].textContent = plural;
        }
    };

    var formatBytePrefix = function (bytes) {
        if (bytes == 0) return '0 B';
        var k = 1000,
            dm = 1,
            sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
            i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
    }

    return {
        formatLabel: formatLabel,
        formatBytePrefix: formatBytePrefix
    };


})();