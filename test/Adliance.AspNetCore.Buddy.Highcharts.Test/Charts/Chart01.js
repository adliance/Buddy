function(chart) {

    // ReSharper disable UseOfImplicitGlobalInFunctionScope

    var primaryColor = "#142855";
    var secondaryColor = "#3EB4B5";
    var greenColor = "#a2a735";
    var showMarkersValue = true;
    var showMarkersHasValue = true;
    var footnotes = [{ "x": 0.0, "text": "WHO (1)" }, { "x": 4.0, "text": "APEDÖ (2)" }];
    var target = { "x": 19.0, "y": 186.5, "text": "Ziellänge: 186,5 cm" };
    var percentileAnnotations = [
        { "x": 19.0, "y": 160.36, "text": "0,13", "color": "#bbb" },
        { "x": 19.0, "y": 163.76, "text": "0,62", "color": "#bbb" },
        { "x": 19.0, "y": 167.78, "text": "3", "color": "#aaa" },
        { "x": 19.0, "y": 171.61, "text": "10", "color": "#999" },
        { "x": 19.0, "y": 175.41, "text": "25", "color": "#888" },
        { "x": 19.0, "y": 179.56, "text": "Median", "color": "#777" },
        { "x": 19.0, "y": 183.73, "text": "75", "color": "#888" },
        { "x": 19.0, "y": 187.58, "text": "90", "color": "#999" },
        { "x": 19.0, "y": 191.47, "text": "97", "color": "#aaa" }
    ];
    var percentileDescriptions = [];
    var markers = [{ "x": 3.0, "y": 100.0, "text": "Heute", "color": "#072C51" }];

    Highcharts.Renderer.prototype.symbols.hline = function (x, y, width, height) {
        console.log("HLINE");
        return ["M", x, y + height / 2, "L", x + width, y + width / 2];
    };
    Highcharts.setOptions({ lang: { resetZoom: "Zoom aufheben" } });

    var showMarkers = showMarkersValue;
    if (!showMarkersHasValue) {
        showMarkers = $(window).width() > 1000;
    }

    console.log("Show Markers", showMarkers);

    chart.tooltip.options.formatter = function () { return this.key };
    chart.xAxis[0].update({
        events: {
            setExtremes: function (arg) {
                handleZoomEvent(!!arg.min || !!arg.max);
            }
        }
    });

    chart.hideOnZoom = new Array();

    // add plot line for separating multiple reference values
    if (footnotes.length > 1 && showMarkers) {
        chart.xAxis[0].addPlotLine({
            color: secondaryColor,
            width: 1,
            value: footnotes[1].x
        });
    }

    // footnote text
    if (showMarkers) {
        for (var i = 0; i < footnotes.length; i++) {
            chart.addAnnotation({
                draggable: "",
                labels: [
                    {
                        point: {
                            x: footnotes[i].x === 0 ? chart.xAxis[0].min : footnotes[i].x,
                            y: chart.yAxis[0].min,
                            xAxis: 0,
                            yAxis: 0
                        },
                        y: -32,
                        x: -2,
                        text: footnotes[i].text,
                        distance: 0,
                        overflow: "none",
                        backgroundColor: "transparent",
                        borderColor: "transparent",
                        align: "left",
                        useHTML: true,
                        verticalAlign: "top",
                        style: {
                            color: secondaryColor,
                            fontSize: "11px"
                        }
                    }
                ]
            });
        }
    }

    // add target length
    if (target) {
        console.log("target", target);
        chart.addSeries({
            name: "Ziellänge",
            type: "scatter",
            color: greenColor,
            zIndex: 1000,
            marker: {
                symbol: "hline",
                lineWidth: showMarkers ? 4 : 0,
                lineColor: greenColor,
                radius: 12
            },
            data: [{ x: target.x, y: target.y, name: target.text }]
        });
    }

    // add annotation text for target length
    if (target && showMarkers) {
        var targetX = chart.xAxis[0].toPixels(target.x);
        var targetY = chart.yAxis[0].toPixels(target.y);
        targetX = targetX - chart.plotLeft + 15;
        targetY = targetY - chart.plotTop + 27;

        chart.hideOnZoom.push(chart.addAnnotation(
            {
                draggable: "",
                labels: [
                    {
                        point: { x: targetX, y: targetY },
                        text: "ZL",
                        overflow: "none",
                        distance: 0,
                        align: "left",
                        verticalAlign: "bottom",
                        backgroundColor: "transparent",
                        borderWidth: 0,
                        style: {
                            color: greenColor,
                            fontSize: "10px"
                        }
                    }
                ]
            }));
    }

    // add annotation text for each percentile name
    if (showMarkers) {
        for (var i = 0; i < percentileAnnotations.length; i++) {
            var percentileEndX = chart.xAxis[0].toPixels(percentileAnnotations[i].x);
            var percentileEndY = chart.yAxis[0].toPixels(percentileAnnotations[i].y);
            percentileEndX = percentileEndX - chart.plotLeft - 3;
            percentileEndY = percentileEndY - chart.plotTop + 4;

            chart.hideOnZoom.push(chart.addAnnotation({
                zIndex: 0,
                draggable: "",
                labels: [
                    {
                        point: {
                            x: percentileEndX + 5,
                            y: percentileEndY + 5
                        },
                        text: percentileAnnotations[i].text,
                        distance: 0,
                        overflow: "none",
                        backgroundColor: "transparent",
                        borderColor: "transparent",
                        align: "left",
                        padding: 0,
                        verticalAlign: "top",
                        style: {
                            color: percentileAnnotations[i].color,
                            fontSize: "9px"
                        }
                    }
                ]
            }));
        }
    }

    // add description for each percentile
    if (showMarkers) {
        for (var i = 0; i < percentileDescriptions.length; i++) {
            var percentileEndX = chart.xAxis[0].toPixels(percentileDescriptions[i].x);
            var percentileEndY = chart.yAxis[0].toPixels(percentileDescriptions[i].y);
            percentileEndX = percentileEndX - chart.plotLeft - 3;
            percentileEndY = percentileEndY - chart.plotTop + 4;

            chart.hideOnZoom.push(chart.addAnnotation({
                draggable: "",
                labels: [
                    {
                        point: {
                            x: percentileEndX,
                            y: percentileEndY
                        },
                        text: percentileDescriptions[i].text,
                        distance: 0,
                        x: -80,
                        y: 15,
                        overflow: "none",
                        backgroundColor: "transparent",
                        borderColor: "transparent",
                        align: "left",
                        verticalAlign: "top",
                        style: {
                            color: percentileDescriptions[i].color,
                            fontSize: "13px"
                        }
                    }
                ]
            }));
        }
    }

    // add markers
    if (showMarkers) {
        for (var i = 0; i < markers.length; i++) {
            chart.addAnnotation({
                draggable: "",
                labels: [
                    {
                        point: {
                            x: markers[i].x,
                            y: markers[i].y,
                            xAxis: 0,
                            yAxis: 0
                        },
                        text: "<span style=\"background-color: rgba(255,255,255,0.85);\">" +
                            markers[i].text +
                            "</span>",
                        borderWidth: 1,
                        allowOverlap: true,
                        crop: false,
                        align: "center",
                        borderColor: markers[i].color,
                        shape: "connector",
                        padding: 0,
                        useHTML: true,
                        y: 150,
                        style: {
                            color: markers[i].color,
                            fontSize: "10px",
                            textAlign: "center"
                        }
                    }
                ]
            });
        }
    }

    function handleZoomEvent(isZoomed) {
        if (!chart.hideOnZoom) {
            return;
        }

        for (var i = 0; i < chart.hideOnZoom.length; i++) {
            var annotation = chart.hideOnZoom[i];

            if (isZoomed) {
                annotation.labels[0].options.originalText = annotation.labels[0].options.text;
                annotation.labels[0].options.text = " ";
            } else {
                annotation.labels[0].options.text = annotation.labels[0].options.originalText;
            }
        }
    }

    // ReSharper restore UseOfImplicitGlobalInFunctionScope
}