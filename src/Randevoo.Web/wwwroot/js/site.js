(() => {
    const toPersianDigits = (value) => String(value)
        .replaceAll("0", "۰")
        .replaceAll("1", "۱")
        .replaceAll("2", "۲")
        .replaceAll("3", "۳")
        .replaceAll("4", "۴")
        .replaceAll("5", "۵")
        .replaceAll("6", "۶")
        .replaceAll("7", "۷")
        .replaceAll("8", "۸")
        .replaceAll("9", "۹");

    document.querySelectorAll("[data-range-filter]").forEach((filter) => {
        const toggle = filter.querySelector("[data-range-toggle]");
        const input = filter.querySelector("[data-range-input]");
        const output = filter.querySelector("[data-range-output]");
        const syncRange = () => {
            if (!input || !output) return;
            output.textContent = toggle?.checked
                ? `${toPersianDigits(input.value)} سال`
                : "همه سنین";
            filter.classList.toggle("is-active", Boolean(toggle?.checked));
        };

        toggle?.addEventListener("change", syncRange);
        input?.addEventListener("input", syncRange);
        syncRange();
    });

    const form = document.querySelector("[data-profile-form]");
    if (!form) return;

    const syncChoiceState = () => {
        form.querySelectorAll(".rv-choice-card, .rv-interest-chip").forEach((label) => {
            const input = label.querySelector("input");
            label.classList.toggle("is-selected", Boolean(input?.checked));
        });
    };

    form.addEventListener("change", (event) => {
        if (event.target.matches(".rv-choice-card input, .rv-interest-chip input")) {
            syncChoiceState();
        }
    });

    const heightSlider = form.querySelector("[data-height-slider]");
    const heightOutput = form.querySelector("[data-height-output]");
    const syncHeight = () => {
        if (heightSlider && heightOutput) {
            heightOutput.textContent = `${toPersianDigits(heightSlider.value)} سانتی‌متر`;
        }
    };
    heightSlider?.addEventListener("input", syncHeight);
    syncHeight();

    const interestInputs = [...form.querySelectorAll("[data-interest-input]")];
    const interestCount = form.querySelector("[data-interest-count]");
    const syncInterestLimit = () => {
        const selected = interestInputs.filter((input) => input.checked);
        if (interestCount) interestCount.textContent = toPersianDigits(selected.length);

        interestInputs.forEach((input) => {
            input.disabled = selected.length >= 4 && !input.checked;
            input.closest(".rv-interest-chip")?.classList.toggle("is-disabled", input.disabled);
        });
    };
    interestInputs.forEach((input) => input.addEventListener("change", syncInterestLimit));
    syncInterestLimit();

    const primaryInput = form.querySelector("[data-primary-photo]");
    const syncPrimaryPhoto = () => {
        const tiles = [...form.querySelectorAll("[data-photo-tile]")];
        let primary = primaryInput?.value;

        if (!primary && tiles.length > 0) {
            primary = tiles[0].querySelector("[data-photo-url]")?.value;
            if (primaryInput) primaryInput.value = primary ?? "";
        }

        tiles.forEach((tile) => {
            const url = tile.querySelector("[data-photo-url]")?.value;
            const isPrimary = Boolean(url && primary && url.toLowerCase() === primary.toLowerCase());
            tile.classList.toggle("is-primary", isPrimary);
            const button = tile.querySelector("[data-set-primary]");
            if (button) {
                button.classList.toggle("btn-primary", isPrimary);
                button.classList.toggle("btn-light", !isPrimary);
            }
        });
    };

    form.addEventListener("click", (event) => {
        const setPrimary = event.target.closest("[data-set-primary]");
        if (setPrimary) {
            const url = setPrimary.closest("[data-photo-tile]")?.querySelector("[data-photo-url]")?.value;
            if (url && primaryInput) {
                primaryInput.value = url;
                syncPrimaryPhoto();
            }
            return;
        }

        const removePhoto = event.target.closest("[data-remove-photo]");
        if (removePhoto) {
            const tile = removePhoto.closest("[data-photo-tile]");
            const removedUrl = tile?.querySelector("[data-photo-url]")?.value;
            tile?.remove();
            if (primaryInput && removedUrl && primaryInput.value.toLowerCase() === removedUrl.toLowerCase()) {
                primaryInput.value = "";
            }
            syncPrimaryPhoto();
        }
    });
    syncPrimaryPhoto();

    const photoInputs = [...form.querySelectorAll("[data-photo-input]")];
    const preview = form.querySelector("[data-photo-preview]");
    photoInputs.forEach((input) => {
        input.addEventListener("change", () => {
            if (!preview) return;
            preview.innerHTML = "";
            [...input.files].slice(0, 3).forEach((file) => {
                if (!file.type.startsWith("image/")) return;
                const item = document.createElement("div");
                item.className = "rv-photo-preview__item";
                const image = document.createElement("img");
                image.alt = "پیش‌نمایش عکس";
                image.src = URL.createObjectURL(file);
                image.addEventListener("load", () => URL.revokeObjectURL(image.src), { once: true });
                item.append(image);
                preview.append(item);
            });
        });
    });

    syncChoiceState();
})();
