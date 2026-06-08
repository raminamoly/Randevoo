(function (window) {
    const chartFont = (window.getComputedStyle
        ? window.getComputedStyle(document.documentElement).getPropertyValue("--rv-font-chart").trim()
        : "") || '"IRANSansXFaNum", "IRANSansX", "Tahoma", "Segoe UI", Arial, sans-serif';
    const palette = ["#1765ff", "#ff3c72", "#f4a621", "#22b573", "#8e24aa", "#ef6c00", "#00a6a6", "#0f1f4d"];

    const toFaNumber = function (value) {
        return new Intl.NumberFormat("fa-IR").format(Number(value || 0));
    };

    const hexToRgba = function (hex, alpha) {
        const clean = String(hex || "#1765ff").replace("#", "");
        const bigint = parseInt(clean.length === 3
            ? clean.split("").map(function (item) { return item + item; }).join("")
            : clean, 16);
        const red = (bigint >> 16) & 255;
        const green = (bigint >> 8) & 255;
        const blue = bigint & 255;
        return `rgba(${red}, ${green}, ${blue}, ${alpha})`;
    };

    const getCanvas = function (canvasId) {
        const canvas = document.getElementById(canvasId);
        if (!canvas || !window.Chart) {
            return null;
        }

        return canvas;
    };

    const makeGradient = function (canvas, color, strongAlpha, softAlpha) {
        const context = canvas.getContext("2d");
        const gradient = context.createLinearGradient(0, 0, 0, canvas.clientHeight || 280);
        gradient.addColorStop(0, hexToRgba(color, strongAlpha));
        gradient.addColorStop(0.65, hexToRgba(color, softAlpha));
        gradient.addColorStop(1, hexToRgba(color, 0));
        return gradient;
    };

    const baseOptions = function (withScales) {
        const options = {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 900,
                easing: "easeOutQuart"
            },
            interaction: {
                mode: "index",
                intersect: false
            },
            plugins: {
                legend: {
                    position: "bottom",
                    align: "center",
                    labels: {
                        boxWidth: 10,
                        boxHeight: 10,
                        usePointStyle: true,
                        padding: 18,
                        font: {
                            family: chartFont,
                            size: 12,
                            weight: "600"
                        }
                    }
                },
                tooltip: {
                    rtl: true,
                    textDirection: "rtl",
                    backgroundColor: "rgba(15, 31, 77, 0.92)",
                    borderColor: "rgba(255, 255, 255, 0.18)",
                    borderWidth: 1,
                    cornerRadius: 14,
                    padding: 12,
                    usePointStyle: true,
                    titleFont: {
                        family: chartFont,
                        size: 12,
                        weight: "600"
                    },
                    bodyFont: {
                        family: chartFont,
                        size: 12,
                        weight: "500"
                    },
                    callbacks: {
                        label: function (context) {
                            const label = context.label || context.dataset.label || "";
                            const value = context.parsed && typeof context.parsed === "object"
                                ? (context.parsed.y ?? context.parsed)
                                : context.parsed;
                            return `${label ? label + ": " : ""}${toFaNumber(value)}`;
                        }
                    }
                }
            }
        };

        if (withScales) {
            options.scales = {
                x: {
                    border: { display: false },
                    grid: { display: false },
                    ticks: {
                        color: "rgba(23, 32, 51, 0.62)",
                        maxRotation: 0,
                        autoSkipPadding: 18,
                        font: { family: chartFont, size: 11, weight: "500" }
                    }
                },
                y: {
                    beginAtZero: true,
                    border: { display: false },
                    grid: {
                        color: "rgba(15, 31, 77, 0.08)",
                        drawTicks: false
                    },
                    ticks: {
                        color: "rgba(23, 32, 51, 0.58)",
                        padding: 10,
                        font: { family: chartFont, size: 11, weight: "500" },
                        callback: toFaNumber
                    }
                }
            };
        }

        return options;
    };

    const configureDefaults = function () {
        if (!window.Chart) {
            return;
        }

        window.Chart.defaults.font.family = chartFont;
        window.Chart.defaults.font.size = 12;
        window.Chart.defaults.color = "rgba(23, 32, 51, 0.72)";
        window.Chart.defaults.borderColor = "rgba(15, 31, 77, 0.08)";
    };

    const buildLine = function (canvasId, points, options) {
        const canvas = getCanvas(canvasId);
        const source = Array.isArray(points) ? points : [];
        if (!canvas || source.length === 0) {
            return null;
        }

        const color = (options && options.color) || palette[0];
        return new window.Chart(canvas, {
            type: "line",
            data: {
                labels: source.map(function (item) { return item.label; }),
                datasets: [{
                    label: (options && options.label) || "",
                    data: source.map(function (item) { return item.value; }),
                    borderColor: color,
                    backgroundColor: makeGradient(canvas, color, 0.28, 0.08),
                    fill: true,
                    tension: 0.42,
                    borderWidth: 3,
                    pointRadius: 0,
                    pointHoverRadius: 5,
                    pointBackgroundColor: "#ffffff",
                    pointBorderColor: color,
                    pointBorderWidth: 3
                }]
            },
            options: Object.assign(baseOptions(true), {
                plugins: Object.assign(baseOptions(true).plugins, {
                    legend: { display: false },
                    tooltip: baseOptions(true).plugins.tooltip
                })
            })
        });
    };

    const buildBar = function (canvasId, points, options) {
        const canvas = getCanvas(canvasId);
        const source = Array.isArray(points) ? points : [];
        if (!canvas || source.length === 0) {
            return null;
        }

        const color = (options && options.color) || palette[0];
        return new window.Chart(canvas, {
            type: "bar",
            data: {
                labels: source.map(function (item) { return item.label; }),
                datasets: [{
                    label: (options && options.label) || "",
                    data: source.map(function (item) { return item.value; }),
                    backgroundColor: makeGradient(canvas, color, 0.78, 0.28),
                    borderColor: color,
                    borderWidth: 1,
                    borderRadius: 10,
                    borderSkipped: false,
                    maxBarThickness: 42
                }]
            },
            options: Object.assign(baseOptions(true), {
                plugins: Object.assign(baseOptions(true).plugins, {
                    legend: { display: false },
                    tooltip: baseOptions(true).plugins.tooltip
                })
            })
        });
    };

    const buildDoughnut = function (canvasId, slices, options) {
        const canvas = getCanvas(canvasId);
        const source = Array.isArray(slices) ? slices : [];
        if (!canvas || source.length === 0) {
            return null;
        }

        return new window.Chart(canvas, {
            type: "doughnut",
            data: {
                labels: source.map(function (item) { return item.label; }),
                datasets: [{
                    data: source.map(function (item) { return item.value; }),
                    backgroundColor: palette,
                    borderColor: "#ffffff",
                    borderWidth: 4,
                    borderRadius: 8,
                    spacing: 3,
                    hoverOffset: 8
                }]
            },
            options: Object.assign(baseOptions(false), {
                cutout: (options && options.cutout) || "62%"
            })
        });
    };

    configureDefaults();

    window.RandevooCharts = {
        palette: palette,
        line: buildLine,
        bar: buildBar,
        doughnut: buildDoughnut
    };
})(window);
