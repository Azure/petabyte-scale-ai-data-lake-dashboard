// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

var Animators = (function () {

    var animateCircle = function (svg, path, radius) {
        var i = 0;
        var angle = 0;
        var angleDelta = 10;

        var centerX = svg.width() / 2;
        var centerY = svg.height() / 2;

        var arcAngle = 90;
        var rotationAngle = 0;

        var rotate = function () {
            var arcAngleRadians = (arcAngle / 180) * Math.PI;
            var rotationAngleRadians = (rotationAngle / 180) * Math.PI;

            var startX = centerX + Math.cos(rotationAngleRadians) * radius;
            var startY = centerY + Math.sin(rotationAngleRadians) * radius;

            var endX = centerX + Math.cos(rotationAngleRadians + arcAngleRadians) * radius;
            var endY = centerY + Math.sin(rotationAngleRadians + arcAngleRadians) * radius;

            path.attr("d",
                "M " + startX + " " + startY +
                "A " + radius + " " + radius + " 0 0 1 " + endX + " " + endY);

            rotationAngle += angleDelta;

            if (rotationAngle > 360) {
                rotationAngle = 0;
            }

            setTimeout(rotate, 50);
        }

        rotate();
    };

    var animateTriSquares = function (svg, squares) {
        var update = function () {
            var previousFill = $(squares[0]).attr("fill");
            for (var r = 1; r < squares.length; r++) {
                var tmp = $(squares[r]).attr("fill");
                $(squares[r]).attr("fill", previousFill);
                previousFill = tmp;
            }

            $(squares[0]).attr("fill", previousFill);
            setTimeout(update, 800);
        };
        update();
    }

    return {
        animateCircle: animateCircle,
        animateTriSquares: animateTriSquares
    };
})();