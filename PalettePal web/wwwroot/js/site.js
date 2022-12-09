let image = undefined;
const maxFileSize = 5242880;

function previewImage(preview, image) {
    const reader = new FileReader();
    reader.onload = () => {
        preview.attr('src', reader.result);
        preview.css('mix-blend-mode', 'unset');
        preview.parent().width('fit-content');
        preview.parent().height('fit-content');
    }
    reader.readAsDataURL(image);
}

function addColor(colorsList, color) {
    let elem = document.createElement('li');
    elem.className = 'color-item-animation';
    elem.style.backgroundColor = color;
    elem.style.color = contrastYiq('0x' + color.substring(1));
    elem.innerHTML = color;
    elem.onclick = () => {
        navigator.clipboard.writeText(color);
        if (elem.textContent != 'Скопировано') {
            elem.textContent = 'Скопировано';
            setTimeout(() => {
                elem.innerText = color;
            }, 2000);
        }
    };
    colorsList.append(elem);
}

function contrastYiq(color) {
    const r = (color >>> 16) & 0xff;
    const g = (color >>> 8) & 0xff;
    const b = color & 0xff;
    const yiq = (r * 299 + g * 587 + b * 114) / 1000;
    return yiq >= 128 ? 'black' : 'white';
};

function prepareDragAndDrop(preview) {
    const dropZone = $('body');
    const blackout = $('#blackout');
    dropZone.on('drag dragstart dragend dragover dragenter dragleave drop', function () {
        return false;
    });
    dropZone.on('dragover dragenter', function () {
        blackout.show();
        if (!image) preview.css('mix-blend-mode', 'color-dodge');
    });
    dropZone.on('dragleave', function (e) {
        let dx = e.pageX - dropZone.offset().left;
        let dy = e.pageY - dropZone.offset().top;
        if ((dx <= 0) || (dx >= dropZone.width()) || (dy <= 0) || (dy >= dropZone.height())) {
            if (!image) preview.css('mix-blend-mode', 'overlay');
            blackout.hide();
        };
    });
    dropZone.on('drop', function (e) {
        blackout.hide();
        setImage(preview, e.originalEvent.dataTransfer.files[0]);
    });
}

function setImage(preview, file) {
    if ((file != undefined) &&
        (file.size <= maxFileSize) &&
        ((file.type == 'image/png') || (file.type == 'image/jpeg'))) {
        image = file;
        previewImage(preview, file);
    }
    else {
        // show error message
    }
}

$(() => {
    const preview = $('#form-image');
    const colorsList = $('.colors-list').first();
    const openBtn = $('#open-btn');
    const fileInput = $('#file-input');
    const submitBorders = $('.thin-border, .thick-border');

    openBtn.click(() => {
        $('#file-input').click();
    });

    preview.parent().click(() => {
        if (!image) $('#file-input').click();
    });

    fileInput.change(() => {
        setImage(preview, document.querySelector('input[type=file]').files[0]);
    });

    prepareDragAndDrop(preview);

    $('form').submit(async function (event) {
        event.preventDefault();
        colorsList.empty();
        if (image) {
            submitBorders.css('opacity', 1);
            let formData = new FormData();
            formData.append('ColorsCount', this.elements.ColorsCount.value);
            formData.append('FormFile', image);
            let response = await fetch(this.action, {
                method: 'ajax',
                body: formData
            });
            let result = await response.json();
            for (let color of result.colorsList) {
                addColor(colorsList, color);
            };
            submitBorders.css('opacity', 0);
            $('.palette').show();
            colorsList[0].scrollIntoView({
                behavior: "smooth"
            });
        }
        else {
            openBtn.addClass('shake-horizontal');
            setTimeout(() => {
                openBtn.removeClass('shake-horizontal');
            }, 600);
        }
    });

    for (var i = 1; i < 9; i++) {
        const delay = +Math.random().toFixed(2) * 2;
        $('#background').append(
            `<div class="ellipse-${i} blinking" style="
animation-duration: 3.5s, ${delay / 2 + 3}s;
animation-delay: ${delay}s, ${delay + 3.5}s"></div>`
        );
    };
})