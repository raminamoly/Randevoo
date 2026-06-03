$(function () {
    $(".quill-editor").each(function () {
        const host = this;
        const inputSelector = host.dataset.input;
        const hiddenInput = inputSelector ? document.querySelector(inputSelector) : null;
        if (!host || !hiddenInput || !window.Quill) {
            return;
        }

        const quill = new Quill(host, {
            theme: "snow",
            placeholder: host.dataset.placeholder || "",
            modules: {
                toolbar: [
                    [{ header: [2, 3, false] }],
                    [{ align: [] }, { direction: "rtl" }],
                    ["bold", "italic", "underline", "strike"],
                    [{ list: "ordered" }, { list: "bullet" }],
                    ["blockquote", "link"],
                    ["clean"]
                ]
            }
        });

        quill.root.innerHTML = hiddenInput.value || "";
        quill.root.setAttribute("dir", "rtl");
        quill.root.setAttribute("lang", "fa");
        quill.format("align", "right");
        quill.format("direction", "rtl");

        const sync = function () {
            hiddenInput.value = quill.root.innerHTML === "<p><br></p>" ? "" : quill.root.innerHTML;
        };

        quill.on("text-change", sync);
        $(host).closest("form").on("submit", sync);
    });

    $(".image-input").on("change", function () {
        const input = this;
        const target = $(input).data("target");
        const file = input.files && input.files[0];

        if (!file || !target) {
            return;
        }

        const reader = new FileReader();
        reader.onload = function (e) {
            const image = $(target);
            image.attr("src", e.target.result).removeClass("d-none");
            image.closest(".image-manager-preview-shell").removeClass("is-empty");
        };
        reader.readAsDataURL(file);
    });

    $(".image-clear-button").on("click", function () {
        const previewSelector = $(this).data("target");
        const hiddenSelector = $(this).data("hidden-target");
        const fileSelector = $(this).data("file-target");

        if (previewSelector) {
            const image = $(previewSelector);
            image.attr("src", "").addClass("d-none");
            image.closest(".image-manager-preview-shell").addClass("is-empty");
        }

        if (hiddenSelector) {
            $(hiddenSelector).val("");
        }

        if (fileSelector) {
            $(fileSelector).val("");
        }
    });

    $("[data-tag-input='true']").each(function () {
        const input = this;
        const previewTarget = input.dataset.tagPreviewTarget ? document.querySelector(input.dataset.tagPreviewTarget) : null;
        if (!previewTarget) {
            return;
        }

        const parseTags = function (value) {
            return (value || "")
                .split(/[,،\n\r]+/)
                .map(function (item) { return item.trim(); })
                .filter(function (item, index, list) { return item.length > 0 && list.indexOf(item) === index; })
                .slice(0, 10);
        };

        const renderTags = function () {
            const tags = parseTags(input.value);
            previewTarget.innerHTML = "";

            if (tags.length === 0) {
                previewTarget.innerHTML = '<span class="tag-preview-empty">هنوز تگی ثبت نشده است.</span>';
                return;
            }

            tags.forEach(function (tag) {
                const chip = document.createElement("span");
                chip.className = "event-chip";
                chip.textContent = tag;
                previewTarget.appendChild(chip);
            });
        };

        input.addEventListener("input", renderTags);
        renderTags();
    });

    if ($.fn.persianDatepicker && $(".jalali-date-picker").length) {
        $(".jalali-date-picker").persianDatepicker({
            format: "YYYY/MM/DD",
            initialValue: false,
            observer: true,
            persianDigit: true,
            toolbox: {
                calendarSwitch: {
                    enabled: false
                }
            }
        });
    }

    if (window.randevooEventEdit && window.L) {
        const config = window.randevooEventEdit;
        const latitudeInput = document.querySelector(config.latitudeSelector);
        const longitudeInput = document.querySelector(config.longitudeSelector);
        const latitudeDisplay = config.latitudeDisplaySelector ? document.querySelector(config.latitudeDisplaySelector) : null;
        const longitudeDisplay = config.longitudeDisplaySelector ? document.querySelector(config.longitudeDisplaySelector) : null;
        const citySelector = document.querySelector(config.citySelector);
        const mapElement = document.getElementById("eventMap");

        if (mapElement && latitudeInput && longitudeInput && citySelector) {
            const cityCenters = {
                "تهران": [35.6892, 51.3890],
                "مشهد": [36.2605, 59.6168],
                "شیراز": [29.5918, 52.5837],
                "اصفهان": [32.6546, 51.6680],
                "تبریز": [38.0962, 46.2738]
            };

            const startLat = parseFloat(latitudeInput.value || "35.7219");
            const startLng = parseFloat(longitudeInput.value || "51.3347");
            const map = L.map(mapElement, { scrollWheelZoom: false }).setView([startLat, startLng], 13);

            L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
                maxZoom: 19,
                attribution: "&copy; OpenStreetMap"
            }).addTo(map);

            const marker = L.marker([startLat, startLng], { draggable: true }).addTo(map);

            function toPersianDigits(value) {
                return String(value)
                    .replace(/0/g, "۰")
                    .replace(/1/g, "۱")
                    .replace(/2/g, "۲")
                    .replace(/3/g, "۳")
                    .replace(/4/g, "۴")
                    .replace(/5/g, "۵")
                    .replace(/6/g, "۶")
                    .replace(/7/g, "۷")
                    .replace(/8/g, "۸")
                    .replace(/9/g, "۹");
            }

            function syncCoordinates(lat, lng) {
                latitudeInput.value = lat.toFixed(4);
                longitudeInput.value = lng.toFixed(4);

                if (latitudeDisplay) {
                    latitudeDisplay.textContent = toPersianDigits(lat.toFixed(4));
                }

                if (longitudeDisplay) {
                    longitudeDisplay.textContent = toPersianDigits(lng.toFixed(4));
                }
            }

            marker.on("dragend", function (event) {
                const position = event.target.getLatLng();
                syncCoordinates(position.lat, position.lng);
            });

            map.on("click", function (event) {
                marker.setLatLng(event.latlng);
                syncCoordinates(event.latlng.lat, event.latlng.lng);
            });

            citySelector.addEventListener("change", function () {
                const selected = cityCenters[citySelector.value];
                if (!selected) {
                    return;
                }

                map.setView(selected, 12);
                marker.setLatLng(selected);
                syncCoordinates(selected[0], selected[1]);
            });

            setTimeout(function () {
                map.invalidateSize();
            }, 200);

            syncCoordinates(startLat, startLng);
        }
    }
});
