const clickBlockFullScreen = (el) => {

    const id = el.getAttribute('data');
    const content = document.getElementById(id);

    content.classList.toggle('mtd-desk-block-content--full');

    if (content.classList.contains('mtd-desk-block-content--full')) {
        setTimeout(() => BodyShowHide(content), 500);
    } else {
        BodyShowHide(content);
    }    
    
}

const clickBlockToogle = (el) => {
    const id = el.getAttribute('data');
    const content = document.getElementById(id);
    content.classList.toggle('mtd-desk-block-content--colapsed');    
}

const BodyShowHide = (content) => {
    content.querySelector(".mtd-desk-block-body--hidden").classList.toggle("mtd-main-display-none");
}


const fabs = document.querySelectorAll(".mtd-fab");
if (fabs) {
    fabs.forEach((fab) => {
        console.log(fab);
        fab.addEventListener("mouseover", () => {
            fab.querySelector(".mdc-fab__label").classList.toggle("mtd-main-display-none");
            fab.classList.toggle("mdc-fab--extended");
        });
        fab.addEventListener("mouseout", () => {
            fab.classList.toggle("mdc-fab--extended");
            fab.querySelector(".mdc-fab__label").classList.toggle("mtd-main-display-none");
        });
    });
}
