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