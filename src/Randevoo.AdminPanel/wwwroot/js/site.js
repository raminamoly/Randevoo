$(function () {
    if (window.bootstrap && window.bootstrap.Tooltip) {
        document.querySelectorAll("[data-bs-toggle='tooltip']").forEach(function (element) {
            const instance = window.bootstrap.Tooltip.getInstance(element);
            if (instance) {
                instance.dispose();
            }

            new window.bootstrap.Tooltip(element);
        });
    }

    if (window.bootstrap && window.bootstrap.Dropdown) {
        const positionFloatingActionMenu = function (toggle, menu) {
            if (!toggle || !menu) {
                return;
            }

            const gap = 8;
            const padding = 12;
            const rect = toggle.getBoundingClientRect();
            const menuWidth = menu.offsetWidth || 220;
            const menuHeight = menu.offsetHeight || 0;
            const alignEnd = menu.classList.contains("dropdown-menu-end");
            const viewportWidth = window.innerWidth || document.documentElement.clientWidth;
            const viewportHeight = window.innerHeight || document.documentElement.clientHeight;

            let left = alignEnd ? rect.right - menuWidth : rect.left;
            left = Math.max(padding, Math.min(left, viewportWidth - menuWidth - padding));

            let top = rect.bottom + gap;
            if (menuHeight > 0 && top + menuHeight > viewportHeight - padding) {
                top = Math.max(padding, rect.top - menuHeight - gap);
            }

            menu.style.position = "fixed";
            menu.style.inset = "auto auto auto auto";
            menu.style.left = `${left}px`;
            menu.style.top = `${top}px`;
            menu.style.right = "auto";
            menu.style.bottom = "auto";
            menu.style.transform = "none";
            menu.style.zIndex = "3000";
        };

        const restoreFloatingActionMenu = function (toggle, menu) {
            const ownerId = toggle && toggle.dataset.actionMenuOwner;
            const owner = ownerId ? document.getElementById(ownerId) : null;

            if (owner && menu && menu.parentElement !== owner) {
                owner.appendChild(menu);
            }

            if (menu) {
                menu.classList.remove("floating-action-menu");
                delete menu.dataset.floatingOwner;
                menu.style.position = "";
                menu.style.inset = "";
                menu.style.left = "";
                menu.style.top = "";
                menu.style.right = "";
                menu.style.bottom = "";
                menu.style.transform = "";
                menu.style.zIndex = "";
            }
        };

        document.querySelectorAll(".table-action-toggle[data-bs-toggle='dropdown']").forEach(function (element, index) {
            const instance = window.bootstrap.Dropdown.getInstance(element);
            if (instance) {
                instance.dispose();
            }

            const dropdown = element.closest(".dropdown");
            const menu = dropdown ? dropdown.querySelector(".dropdown-menu") : null;
            if (dropdown && menu && !dropdown.id) {
                dropdown.id = `table-action-dropdown-${Date.now()}-${index}`;
            }

            if (dropdown) {
                element.dataset.actionMenuOwner = dropdown.id;
            }

            new window.bootstrap.Dropdown(element, {
                display: "static",
                autoClose: true
            });

            element.addEventListener("show.bs.dropdown", function () {
                const currentMenu = dropdown ? dropdown.querySelector(".dropdown-menu") : null;
                if (!currentMenu) {
                    return;
                }

                currentMenu.classList.add("floating-action-menu");
                currentMenu.dataset.floatingOwner = dropdown.id;
                document.body.appendChild(currentMenu);
                requestAnimationFrame(function () {
                    positionFloatingActionMenu(element, currentMenu);
                });
            });

            element.addEventListener("shown.bs.dropdown", function () {
                const currentMenu = document.body.querySelector(`.floating-action-menu.show[data-floating-owner="${dropdown.id}"]`);
                positionFloatingActionMenu(element, currentMenu);
            });

            element.addEventListener("hidden.bs.dropdown", function () {
                const currentMenu = document.body.querySelector(`.floating-action-menu[data-floating-owner="${dropdown.id}"]`);
                restoreFloatingActionMenu(element, currentMenu);
            });
        });

        window.addEventListener("resize", function () {
            const openToggle = document.querySelector(".table-action-toggle.show");
            const openMenu = document.body.querySelector(".floating-action-menu.show");
            positionFloatingActionMenu(openToggle, openMenu);
        });

        window.addEventListener("scroll", function () {
            const openToggle = document.querySelector(".table-action-toggle.show");
            const openMenu = document.body.querySelector(".floating-action-menu.show");
            positionFloatingActionMenu(openToggle, openMenu);
        }, true);
    }

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
        const editor = this;
        const hiddenInput = editor.dataset.tagHiddenTarget ? document.querySelector(editor.dataset.tagHiddenTarget) : null;
        const previewTarget = editor.dataset.tagPreviewTarget ? document.querySelector(editor.dataset.tagPreviewTarget) : null;
        const entryInput = editor.dataset.tagEntryTarget ? document.querySelector(editor.dataset.tagEntryTarget) : null;
        const addButton = editor.querySelector("[data-tag-add-button='true']");
        if (!hiddenInput || !previewTarget || !entryInput) {
            return;
        }

        const normalizeTag = function (value) {
            return (value || "").trim();
        };

        const parseTags = function (value) {
            return (value || "")
                .split(/[,،\n\r]+/)
                .map(normalizeTag)
                .filter(function (item, index, list) {
                    return item.length > 0
                        && list.findIndex(function (candidate) { return candidate.toLowerCase() === item.toLowerCase(); }) === index;
                })
                .slice(0, 10);
        };

        let tags = parseTags(hiddenInput.value);

        const syncHiddenInput = function () {
            hiddenInput.value = tags.join("، ");
        };

        const addTag = function (rawValue) {
            const candidates = parseTags(rawValue);
            let hasChanges = false;

            candidates.forEach(function (candidate) {
                if (tags.length >= 10) {
                    return;
                }

                const exists = tags.some(function (item) {
                    return item.toLowerCase() === candidate.toLowerCase();
                });

                if (!exists) {
                    tags.push(candidate);
                    hasChanges = true;
                }
            });

            if (hasChanges) {
                syncHiddenInput();
                renderTags();
            }
        };

        const removeTag = function (tagToRemove) {
            tags = tags.filter(function (item) {
                return item.toLowerCase() !== tagToRemove.toLowerCase();
            });

            syncHiddenInput();
            renderTags();
        };

        const renderTags = function () {
            previewTarget.innerHTML = "";

            if (tags.length === 0) {
                previewTarget.innerHTML = '<span class="tag-preview-empty">هنوز تگی ثبت نشده است.</span>';
                return;
            }

            tags.forEach(function (tag) {
                const chip = document.createElement("span");
                chip.className = "event-chip event-chip-editable";

                const label = document.createElement("span");
                label.textContent = tag;
                chip.appendChild(label);

                const removeButton = document.createElement("button");
                removeButton.type = "button";
                removeButton.className = "event-chip-remove";
                removeButton.setAttribute("aria-label", "حذف تگ");
                removeButton.textContent = "×";
                removeButton.addEventListener("click", function () {
                    removeTag(tag);
                });
                chip.appendChild(removeButton);

                previewTarget.appendChild(chip);
            });
        };

        entryInput.addEventListener("keydown", function (event) {
            if (event.key === "Enter" || event.key === "," || event.key === "،") {
                event.preventDefault();
                addTag(entryInput.value);
                entryInput.value = "";
            }
        });

        entryInput.addEventListener("blur", function () {
            if (!entryInput.value.trim()) {
                return;
            }

            addTag(entryInput.value);
            entryInput.value = "";
        });

        if (addButton) {
            addButton.addEventListener("click", function () {
                addTag(entryInput.value);
                entryInput.value = "";
                entryInput.focus();
            });
        }

        syncHiddenInput();
        renderTags();
    });

    $("[data-tag-combo='true']").each(function () {
        const editor = this;
        const select = editor.dataset.tagSelectTarget ? document.querySelector(editor.dataset.tagSelectTarget) : null;
        const picker = editor.dataset.tagPickerTarget ? document.querySelector(editor.dataset.tagPickerTarget) : null;
        const previewTarget = editor.dataset.tagPreviewTarget ? document.querySelector(editor.dataset.tagPreviewTarget) : null;

        if (!select || !picker || !previewTarget) {
            return;
        }

        const getSelectedOptions = function () {
            return Array.from(select.options).filter(function (option) {
                return option.selected;
            });
        };

        const syncPickerOptions = function () {
            const selectedValues = new Set(getSelectedOptions().map(function (option) {
                return option.value;
            }));

            Array.from(picker.options).forEach(function (option) {
                if (!option.value) {
                    return;
                }

                option.hidden = selectedValues.has(option.value);
                option.disabled = selectedValues.has(option.value);
            });

            picker.value = "";
        };

        const renderSelectedTags = function () {
            previewTarget.innerHTML = "";
            const selectedOptions = getSelectedOptions();

            if (selectedOptions.length === 0) {
                previewTarget.innerHTML = '<span class="tag-preview-empty">هنوز تگی انتخاب نشده است.</span>';
                syncPickerOptions();
                return;
            }

            selectedOptions.forEach(function (option) {
                const chip = document.createElement("span");
                chip.className = "event-chip event-chip-editable";

                const label = document.createElement("span");
                label.textContent = option.textContent;
                chip.appendChild(label);

                const removeButton = document.createElement("button");
                removeButton.type = "button";
                removeButton.className = "event-chip-remove";
                removeButton.setAttribute("aria-label", `حذف تگ ${option.textContent}`);
                removeButton.innerHTML = '<i class="bi bi-x" aria-hidden="true"></i>';
                removeButton.addEventListener("click", function () {
                    option.selected = false;
                    select.dispatchEvent(new Event("change", { bubbles: true }));
                });
                chip.appendChild(removeButton);

                previewTarget.appendChild(chip);
            });

            syncPickerOptions();
        };

        picker.addEventListener("change", function () {
            if (!picker.value) {
                return;
            }

            const option = Array.from(select.options).find(function (candidate) {
                return candidate.value === picker.value;
            });

            if (option) {
                option.selected = true;
            }

            select.dispatchEvent(new Event("change", { bubbles: true }));
        });

        select.addEventListener("change", renderSelectedTags);
        renderSelectedTags();
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
        const countrySelector = config.countrySelector ? document.querySelector(config.countrySelector) : null;
        const latitudeInput = document.querySelector(config.latitudeSelector);
        const longitudeInput = document.querySelector(config.longitudeSelector);
        const latitudeDisplay = config.latitudeDisplaySelector ? document.querySelector(config.latitudeDisplaySelector) : null;
        const longitudeDisplay = config.longitudeDisplaySelector ? document.querySelector(config.longitudeDisplaySelector) : null;
        const citySelector = document.querySelector(config.citySelector);
        const mapElement = document.getElementById("eventMap");

        if (mapElement && latitudeInput && longitudeInput && citySelector) {
            const cityOptions = Array.isArray(config.cityOptions) ? config.cityOptions : [];

            const findSelectedCity = function () {
                return cityOptions.find(function (city) {
                    return city.name === citySelector.value || city.Name === citySelector.value;
                });
            };

            const getCityName = function (city) {
                return city.name || city.Name;
            };

            const getCountryName = function (city) {
                return city.countryName || city.CountryName;
            };

            const getLatitude = function (city) {
                return parseFloat(city.latitude ?? city.Latitude);
            };

            const getLongitude = function (city) {
                return parseFloat(city.longitude ?? city.Longitude);
            };

            const renderCityOptions = function () {
                if (!countrySelector || cityOptions.length === 0) {
                    return;
                }

                const selectedCountry = countrySelector.value;
                const previousCity = citySelector.value;
                const filteredCities = cityOptions.filter(function (city) {
                    return getCountryName(city) === selectedCountry;
                });

                citySelector.innerHTML = "";
                filteredCities.forEach(function (city) {
                    const option = document.createElement("option");
                    option.value = getCityName(city);
                    option.textContent = getCityName(city);
                    citySelector.appendChild(option);
                });

                const hasPrevious = filteredCities.some(function (city) {
                    return getCityName(city) === previousCity;
                });
                citySelector.value = hasPrevious ? previousCity : (filteredCities[0] ? getCityName(filteredCities[0]) : "");
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
                const selected = findSelectedCity();
                if (!selected) {
                    return;
                }

                const selectedLat = getLatitude(selected);
                const selectedLng = getLongitude(selected);
                map.setView([selectedLat, selectedLng], 12);
                marker.setLatLng([selectedLat, selectedLng]);
                syncCoordinates(selectedLat, selectedLng);
            });

            if (countrySelector) {
                countrySelector.addEventListener("change", function () {
                    renderCityOptions();
                    citySelector.dispatchEvent(new Event("change"));
                });
            }

            setTimeout(function () {
                map.invalidateSize();
            }, 200);

            renderCityOptions();
            syncCoordinates(startLat, startLng);
        }
    }
});
