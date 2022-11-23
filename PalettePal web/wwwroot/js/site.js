let colorsContainer = $(".colors-list").first();

$('#open-btn').click(() => {
    console.log("clicked")
    $('#file-input').click();
});

$("form").submit(async function (event) {
    event.preventDefault();
    // TODO: test image size
    colorsContainer.empty();
    let response = await fetch("post", {
        method: "ajax",
        body: new FormData(this)
    });
    let result = await response.json();
    for (let color of result.colorsList) {
        colorsContainer.append(`<div class="color" style="background-color: #${color}"><a>#${color}</a></div>`);
    };
});

function previewFile() {
    const preview = document.querySelector('img');
    const file = document.querySelector('input[type=file]').files[0];
    const reader = new FileReader();

    reader.onload = () => {
        preview.src = reader.result;
    }

    if (file) reader.readAsDataURL(file);
}