
var dragSrcEl = null;
var dragVaule = false;

function handleDragStart(e) {
    //this.className = 'mdc-list-item';
    dragVaule = this.attributes.getNamedItem('aria-checked').nodeValue;
    dragSrcEl = this;
    e.dataTransfer.setDragImage(new Image(), 0, 0);
    e.dataTransfer.effectAllowed = 'move';
    e.dataTransfer.setData('text/html', this.outerHTML);
}
function handleDragOver(e) {
    if (e.preventDefault) {
        e.preventDefault();
    }
    this.classList.add("over");
    e.dataTransfer.dropEffect = 'move';
    return false;
}

function handleDragEnter(e) {
    // over item
}

function handleDragLeave(e) {
    this.classList.remove('over');
}

function handleDrop(e) {

    if (e.stopPropagation) {
        e.stopPropagation();
    }

    if (dragSrcEl != this) {

        this.parentNode.removeChild(dragSrcEl);
        var dropHTML = e.dataTransfer.getData('text/html');
        this.insertAdjacentHTML('beforebegin', dropHTML);
        var dropElem = this.previousSibling;
        dropElem.className = "mdc-list-item";
        new mdc.ripple.MDCRipple(dropElem);
        addDnDHandlers(dropElem);
        if (dragVaule === "true") {
            dropElem.click();
        }

    }
    this.classList.remove('over');
    return false;
}

function handleDragEnd(e) {
    this.classList.remove('over');

    let strData = "";
    const list = document.getElementById("configListFields");
    const clicker = document.getElementById("configListSeq");
    const data = document.getElementById("fieldSeqData");

    list.querySelectorAll('[data-value]').forEach((item) => {
        const d = item.getAttribute("data-value");
        strData += `${d}&`;
    });

    data.value = strData;
    clicker.click();
}



function addDnDHandlers(elem) {
    elem.addEventListener('dragstart', handleDragStart, false);
    elem.addEventListener('dragenter', handleDragEnter, false)
    elem.addEventListener('dragover', handleDragOver, false);
    elem.addEventListener('dragleave', handleDragLeave, false);
    elem.addEventListener('drop', handleDrop, false);
    elem.addEventListener('dragend', handleDragEnd, false);
}




//Start
    const selectPart = new mdc.select.MDCSelect(document.getElementById("configSelectPart"));

    selectPart.listen('MDCSelect:change', (e) => {
        ActionShowModal(true);
        const form = document.getElementById("configPartSelector");
        const curr = document.getElementById("configCurrentPart");
        curr.value = selectPart.value;
        form.submit();
    });

    var cols = document.querySelectorAll('#configListFields .mdc-list-item');
    [].forEach.call(cols, addDnDHandlers);
