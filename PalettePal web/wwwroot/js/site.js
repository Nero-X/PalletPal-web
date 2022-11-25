function previewFile() {
    const preview = document.querySelector('img');
    const file = document.querySelector('input[type=file]').files[0];
    const reader = new FileReader();

    reader.onload = () => {
        preview.src = reader.result;
        $(preview.parentElement).width('fit-content');
        $(preview.parentElement).height('fit-content');
    }

    if (file) reader.readAsDataURL(file);
}

$(() => {
    let colorsContainer = $('.colors-list').first();

    $('#open-btn').click(() => {
        $('#file-input').click();
    });

    $('#file-input').change(previewFile);

    $('form').submit(async function (event) {
        event.preventDefault();
        // TODO: test image size
        colorsContainer.empty();
        let response = await fetch(this.action, {
            method: 'ajax',
            body: new FormData(this)
        });
        let result = await response.json();
        for (let color of result.colorsList) {
            colorsContainer.append(`<li style="background-color: #${color}">#${color}</li>`);
        };
    });

    for (var i = 1; i < 9; i++) {
        $('#background').append(`<div class="ellipse-${i}"></div>`);
    };
})