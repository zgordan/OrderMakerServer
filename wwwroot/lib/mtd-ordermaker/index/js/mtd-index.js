/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/

const getIdRow = (row) => {
    return row.attributes.getNamedItem("mtd-data-row").nodeValue;
}

const getInputForId = (id) => {
    return document.getElementById(`${id}-checkbox`);
}

const IndexShowModal = (show = true, indicator = true) => {

    const fab = document.getElementById("indexCreator");
    const modal = document.getElementById("indexModal");
    const progress = document.getElementById("indexProgress");

    modal.style.display = show ? "" : "none";
    if (fab) { fab.style.display = show ? "none" : "";}    
    progress.style.display = indicator ? "" : "none";
}

const ListenerPageMenu = () => {

    const indexPageMenu = new mdc.menu.MDCMenu(document.getElementById('indexPageMenu'));
    const pb = document.querySelector('[mtd-data-page]');
    const idForm = pb.attributes.getNamedItem('mtd-data-page').nodeValue;
    pb.addEventListener('click', () => {
        indexPageMenu.open = true;
    });

    const ms = document.getElementById("indexMenuSize");
    ms.addEventListener('click', (e) => {
        const pages = e.target.getAttribute("data-value");
        document.body.scrollTop = document.documentElement.scrollTop = 0;
        IndexShowModal();
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", `/api/index/${idForm}/pagesize/${pages}`, true);

        xmlHttp.send();
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                //document.location.reload(location.hash, '');
                document.location.reload();
            }
        }
    });
}

const ListenerFilter = () => {    
    const selectField = new mdc.select.MDCSelect(document.getElementById("indexSelectField"));
    const selectTerm = new mdc.select.MDCSelect(document.getElementById("indexSelectTerm"));
    const selectFilter = new mdc.textField.MDCTextField(document.getElementById("indexSelectFilter"));
    const selectBlock = document.getElementById(`indexSelectBlock`);
    const inputField = document.getElementById("indexInputField"); 
    const selectPeriod = document.getElementById("indexSelectPeriod"); 

    const indexFormSelector = document.getElementById("indexFormSelector");
    const buttonApply = document.getElementById("indexButtonApply");
    const buttonsCancel = document.querySelectorAll("[mtd-index-cancel]");
    const buttonFilterAdd = document.getElementById("indexButtonFilterAdd");
    const selectLists = document.querySelectorAll(`[mtd-data-list]`);

    selectLists.forEach((item) => {
        const id = item.attributes.getNamedItem("mtd-data-list").nodeValue;
        this[`select${id}`] = new mdc.select.MDCSelect(document.getElementById(`${id}-indexlist`));
    });

    selectField.listen('MDCSelect:change', (e) => {
        e.preventDefault();
        let selectedValue = selectField.value;
        selectBlock.style.display = "flex";
        selectPeriod.hidden = true;

        if (selectedValue === "period") {
            selectedValue = "period";
            selectBlock.style.display = "none";
            selectPeriod.hidden = false;
        }

        const li = document.getElementById(`${selectedValue}`);
        const typeField = li.getAttribute("data-type");
        const idField = li.getAttribute("data-value");     
        const input = document.getElementById("indexInputFilter");        
        inputField.value = idField;

        input.placeholder = "";
        input.step = "1";
        input.value = "";

        selectLists.forEach((item) => {
            const id = item.attributes.getNamedItem("mtd-data-list").nodeValue;
            this[`select${id}`].value = "";
            item.style.display = "none";
        });



        switch (typeField) {
            case "1":
            case "4":
            case "9": {
                input.type = "text";
                break;
            }
            case "11": {
                input.type = "text";
                selectTerm.value = 1;
                const forOpen = document.getElementById(`${idField}-indexlist`);
                forOpen.style.display = "";
                selectBlock.style.display = "none";
                break;
            }
            case "2": { input.type = "number"; input.step = "1"; break; }
            case "3": { input.type = "number"; input.step = "0.01"; break; }
            case "12": { input.type = "number"; input.placeholder = "1 or 0"; break; }

            case "5": { input.type = "date"; break; }
            case "6": { input.type = "datetime-local"; break; }
            default: {
                break;
            }
        };

        selectField.valid = true;
        input.focus();
    });

    selectTerm.listen('MDCSelect:change', () => {
        selectTerm.valid = true;
    });

    buttonsCancel.forEach((buttonCancel) => {
        buttonCancel.addEventListener('click', () => {
            indexFormSelector.style.display = "none";
            IndexShowModal(false, false);
        })
    });

    buttonApply.addEventListener('click', () => {

        if (!selectField.value) { selectField.valid = false; return false; }
        if (!selectTerm.value) { selectTerm.valid = false; return false; }
        if (!selectFilter.valid) { return false };

        indexFormSelector.style.display = "none";
        IndexShowModal();

        const form = document.getElementById("indexFormFilter");
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", form.getAttribute("action"), true);

        var formData = new FormData();

        for (var i = 0; i < form.length; i++) {
            formData.append(form[i].name, form[i].value);
        }

        xmlHttp.send(formData);
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                document.location.reload(location.hash, '');
            }
        }

        //const filter = document.getElementById("indexInputFilter");

    });

    if (buttonFilterAdd) {
        buttonFilterAdd.addEventListener('click', () => {
            indexFormSelector.style.display = "block";
            IndexShowModal(true, false);
        });
    }


    const form = document.getElementById("indexFormFilter");
    form.addEventListener('keydown', (e) => {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            buttonApply.click();
            return false;
        }
    });

    const selectPart = new mdc.select.MDCSelect(document.getElementById("indexSelectPart"));
    selectPart.listen('MDCSelect:change', (e) => {
        const indexListColumn = document.getElementById('indexListColumn');
        const items = indexListColumn.querySelectorAll('li');
        items.forEach((item) => {
            const part = item.attributes.getNamedItem("data-part").nodeValue;
            if (part === selectPart.value || selectPart.value === "all") {
                item.style.display = "block";
            } else item.style.display = "none";
        });
    });

    const indexButtonColumnEdit = document.getElementById("indexButtonColumnEdit");
    const indexFilterColumn = document.getElementById("indexFilterColumn");
    const indexButtonColCancel = document.getElementById("indexButtonColCancel");

    indexButtonColCancel.addEventListener('click', () => {
        indexFilterColumn.style.display = "none";
        IndexShowModal(false, false);
    })

    indexButtonColumnEdit.addEventListener('click', () => {
        IndexShowModal(true, false);
        indexFilterColumn.style.display = "block";
    });

    document.getElementById("indexButtonColApply").addEventListener('click', (e) => {
        const indexListColumn = document.getElementById('indexListColumn');
        const items = indexListColumn.querySelectorAll('li');
        let result = "";
        items.forEach((item) => {
            const state = item.attributes.getNamedItem("aria-checked").nodeValue;
            if (state === 'true') {
                result += `${item.attributes.getNamedItem("data-value").nodeValue},`;
            }
        });
        document.getElementById("indexDataColumnList").value = result;
        const form = document.getElementById("indexFormColumn");

        const checkBoxNumber = document.getElementById("show-number");
        const checkBoxDate = document.getElementById("show-date");
        const indexDataColumnNumber = document.getElementById("indexDataColumnNumber");
        const indexDataColumnDate = document.getElementById("indexDataColumnDate");
        indexDataColumnNumber.value = checkBoxNumber.checked;
        indexDataColumnDate.value = checkBoxDate.checked;

        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", form.getAttribute("action"), true);

        indexFilterColumn.style.display = "none";
        IndexShowModal(true, true);

        var formData = new FormData();

        for (var i = 0; i < form.length; i++) {
            formData.append(form[i].name, form[i].value);
        }

        xmlHttp.send(formData);
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                document.location.reload(location.hash, '');
            }
        }

    });
}

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
}

function addDnDHandlers(elem) {
    elem.addEventListener('dragstart', handleDragStart, false);
    elem.addEventListener('dragenter', handleDragEnter, false)
    elem.addEventListener('dragover', handleDragOver, false);
    elem.addEventListener('dragleave', handleDragLeave, false);
    elem.addEventListener('drop', handleDrop, false);
    elem.addEventListener('dragend', handleDragEnd, false);
}

var cols = document.querySelectorAll('#indexListColumn .mdc-list-item');
[].forEach.call(cols, addDnDHandlers);



(() => {

    const rows = document.querySelectorAll("tr[mtd-data-row]");
    rows.forEach((row) => {

        row.addEventListener('click', (e) => {
            if (e.target.type === 'checkbox') return false;
            ClickEventRow(row);
        });
        const id = getIdRow(row);
        const input = getInputForId(id);

        input.addEventListener('click', (e) => {
            e.target.checked = !e.target.checked;
            ClickEventRow(row);
        });

    });

    const listItems = document.querySelectorAll('.mdc-list');
    listItems.forEach((item) => { new mdc.list.MDCList(item); });

    ListenerPageMenu();
    ListenerFilter();


    const tabBar = document.querySelector('.mdc-tab-bar');
    new mdc.tabBar.MDCTabBar(tabBar);
    const tabFilterFields = document.getElementById("tab-filter-fields");
    const formFilterFields = document.getElementById("indexFormFilter");
    const tabFilterCustom = document.getElementById("tab-filter-custom");
    const formFilterCustom = document.getElementById("indexFormCustom");

    tabFilterFields.addEventListener('click', () => {        
        formFilterFields.hidden = false;
        formFilterCustom.hidden = true;
        
    });

    tabFilterCustom.addEventListener('click', () => {        
        formFilterFields.hidden = true;
        formFilterCustom.hidden = false;       
    });

})();