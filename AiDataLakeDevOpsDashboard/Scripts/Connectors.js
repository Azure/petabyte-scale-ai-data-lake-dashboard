// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

var Connectors = (
    function () {
        var sgn = function(x) {
            return (x > 0) ? 1 : -1;
        };

        var createVericalConnector = function (svg, path, startX, startY, endX, endY, label) {

            var deltaX = endX - startX;
            var deltaY = endY - startY;

            var arrowHeight = 12;
            var radius = 0.05 * sgn(deltaX) * deltaX;
            var split = 0.3;

            var arc1 = (startY > endY) ? ((startX < endX) ? 1 : 0) : (startX > endX ? 1 : 0);
            var arc2 = (startY > endY) ? ((startX > endX) ? 1 : 0) : (startX < endX ? 1 : 0);

            var lineStartY = 0;
            if (path.attr("marker-start") !== undefined) {
                lineStartY = startY + arrowHeight * sgn(deltaY);
            }
            else {
                lineStartY = startY;
            }

            path.attr("d", "M" + startX + " " + startY + // move to starting point where the start marker will be
                            "M" + startX + " " + lineStartY + // move to where the line will start
                            " V" + (startY + split * deltaY) + // move vertically
                            " A" + radius + " " + radius + " 0 0 " + arc1 + " " + (startX + radius * sgn(deltaX)) + " " + (startY + split * deltaY + radius * sgn(deltaY)) + // arc
                            " H" + (endX - radius * sgn(deltaX)) + // move horizontally
                            " A" + radius + " " + radius + " 0 0 " + arc2 + " " + endX + " " + (endY - (1 - split) * deltaY + 2 * radius * sgn(deltaY)) + // arc
                            " V" + (endY - arrowHeight * sgn(deltaY))); // move vertically

            // place the label
            if (label !== undefined) {
                var labelX = startX + arrowHeight;
                var labelY = startY + split * deltaY;
                var totalHeight = 0;
                label.children('tspan').each(function (index, childElement) {
                    $(childElement).attr("x", labelX);
                    totalHeight += 20;
                });

                label.attr("x", labelX);
                label.attr("y", labelY);
            }
        };

        var createHorizontalConnector = function (svg, path, startX, startY, endX, endY, label) {

            var deltaX = endX - startX;
            var deltaY = endY - startY;

            var arc1 = (startY < endY) ? 1 : 0;
            var arc2 = (startY > endY) ? 1 : 0;

            var radius = deltaY == 0 ? 0 : 0.05 * deltaX * sgn(deltaX);
            var split = 0.4;
            var arrowWidth = 12;

            path.attr("d", "M" + startX + " " + startY + // starting point
                            " H" + (startX + split * deltaX - radius) + // move horizontally
                            " A" + radius + " " + radius + " 0 0 " + arc1 + " " + (startX + split * deltaX) + " " + (startY + radius * sgn(deltaY)) + // arc
                            " V" + (endY - radius * sgn(deltaY)) + // move vertically
                            " A" + radius + " " + radius + " 0 0 " + arc2 + " " + (startX + split * deltaX + radius) + " " + endY + //arc
                            " H" + (endX - arrowWidth)); // move horizontally to the destination

            // place the label
            if (label !== undefined) {
                var labelX = startX + split * deltaX + 2 * radius;
                var labelHeight = 0;
                label.children('tspan').each(function (index, childElement) {
                    $(childElement).attr("x", labelX);
                    labelHeight += 20;
                });

                label.attr("x", labelX);
                label.attr("y", startY + split * deltaY / 2 - labelHeight / 2);
            }
        };

        var connectVertically = function (svg, path, startElem, endElem, label) {
            
            var svgTop = svg.offset().top;
            var svgLeft = svg.offset().left;

            var startCoord = startElem.offset();
            var endCoord = endElem.offset();

            var startX = startCoord.left + 0.5 * startElem.outerWidth() - svgLeft; 
            var endX = endCoord.left + 0.5 * endElem.outerWidth() - svgLeft;

            var startY = 0, endY = 0;
            if (startElem.offset().top > endElem.offset().top) {
                startY = startCoord.top - svgTop;
                endY = endCoord.top + endElem.outerHeight() - svgTop;
            } else {
                startY = startCoord.top + startElem.outerHeight() - svgTop;
                endY = endCoord.top - svgTop;
            }

            createVericalConnector(svg, path, startX, startY, endX, endY, label);
        };

        var connectHorizontally = function (svg, path, startElem, endElem, label) {

            var svgTop = svg.offset().top;
            var svgLeft = svg.offset().left;

            var startCoord = startElem.offset();
            var endCoord = endElem.offset();

            var startX = startCoord.left + startElem.outerWidth() - svgLeft;
            var startY = startCoord.top + 0.5 * startElem.outerHeight() - svgTop;

            var endX = endCoord.left - svgLeft;
            var endY = endCoord.top + 0.5 * endElem.outerHeight() - svgTop;

            createHorizontalConnector(svg, path, startX, startY, endX, endY, label);
        };

        var resize = function (svg, container) {
            svg.attr("width", container.width());
            svg.attr("height", container.height());
        }

        return {
            connectVertically: connectVertically,
            connectHorizontally: connectHorizontally,
            resize: resize
        };
    })();