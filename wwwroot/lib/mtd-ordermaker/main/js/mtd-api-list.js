
const StoreListRequest = (listId) => {

    ActionShowModal(true);

    const form = document.getElementById(`${listId}-form`);
    const formData = CreateFormData(form);

    fetch(form.action, { method: form.method, body: formData })
        .then((response) => {
            return response.json();
        }).then((data) => {
            console.log(data.imgSrc);
            document.getElementById(`${listId}-form-img`).value = data.formImg;

            StoreListCreateTable(data, listId);
            ActionShowModal(false);

        });
}


const StoreListCreateTable = (data, listId) => {

    const area = document.getElementById(`${listId}-table-area`);
    area.innerHTML = '';

    var table = document.createElement('table');
    var thead = document.createElement('thead');
    table.appendChild(thead);
    var trh = document.createElement('tr');
    thead.appendChild(trh);

    for (var i = 0; i < data.columns.length; i++) {
        var th = document.createElement('th');
        var text = document.createTextNode(data.columns[i]);
        th.appendChild(text);
        trh.appendChild(th);
    }

    thead.appendChild(trh);
    table.appendChild(thead);
    var tbody = document.createElement('tbody');
    table.appendChild(tbody);

    var count = data.store.count > 10 ? 10 : data.store.length;

    for (var r = 0; r < count; r++) {
        var tr = document.createElement('tr');

        tr.addEventListener("click", (e) => {
            var t = e.path[1];
            var storeId = t.cells[0].innerText;

            StoreListClickRow(listId, storeId, data.formImg, data.formName, t.cells[1].innerText);
        });

        for (var c = 0; c < data.columns.length; c++) {
            var td = document.createElement('td');
            var text = document.createTextNode(data.store[r].fields[c].value);

            td.appendChild(text);
            tr.appendChild(td);
        }

        tbody.appendChild(tr);
    }

    var pageLine = document.getElementById(`${listId}-page-line`);
    pageLine.innerText = data.pageLine;
    var pageNumber = document.getElementById(`${listId}-page-number`);
    pageNumber.value = data.pageNumber;

    var pageCount = document.getElementById(`${listId}-page-count`);
    pageCount.value = data.pageCount;

    table.appendChild(tbody);
    area.appendChild(table);
}

const StoreListPageNavFirst = (listId) => {
    var pageNumber = document.getElementById(`${listId}-page-number`);
    if (pageNumber.value === "1") { return false; }
    pageNumber.value = 1;
    fireEvent(pageNumber, "change");
    StoreListRequest(listId);
}

const StoreListPageNavPrev = (listId) => {
    var pageNumber = document.getElementById(`${listId}-page-number`);
    if (pageNumber.value === "1") { return false; }

    var value = parseInt(pageNumber.value);
    pageNumber.value = --value;
    StoreListRequest(listId);
}

const StoreListPageNavNext = (listId) => {
    var pageNumber = document.getElementById(`${listId}-page-number`);
    var pageCount = document.getElementById(`${listId}-page-count`);
    if (pageNumber.value === pageCount.value) { return false; }

    var value = parseInt(pageNumber.value);
    pageNumber.value = ++value;
    StoreListRequest(listId);
}

const StoreListPageNavLast = (listId) => {
    var pageNumber = document.getElementById(`${listId}-page-number`);
    var pageCount = document.getElementById(`${listId}-page-count`);
    if (pageNumber.value === pageCount.value) { return false; }

    pageNumber.value = pageCount.value;
    StoreListRequest(listId);
}

const StoreListClickRow = (listId, storeId, imgSrc, formName, docName) => {

    document.getElementById(`${listId}-selected-id`).value = storeId;
    UpdateViewer(listId, formName, docName)
    UpdateTarget(listId, storeId);

}

const StoreListClearSelected = (listId) => {

    document.getElementById(`${listId}-selected-id`).value = "";

    UpdateViewer(listId);
    UpdateTarget(listId, "");
}


const StoreListClearFilter = (listId) => {

    document.getElementById(`${listId}-search-number`).value = "";
    document.getElementById(`${listId}-search-text`).value = "";
    document.getElementById(`${listId}-page-number`).value = "1";

    StoreListRequest(listId);
}

const UpdateViewer = (listId, formName, docName) => {

    var src = document.getElementById(`${listId}-form-img`).value;
    document.getElementById(`${listId}-viewer-img`).src = src;
    document.getElementById(`${listId}-viewer-form-name`).innerText = formName === undefined ? "" : formName;
    document.getElementById(`${listId}-viewer-doc-name`).innerText = docName === undefined ? "" : docName;

}

const UpdateTarget = (listId, result) => {

    const targetId = document.getElementById(`${listId}-selected-target`).value;
    document.getElementById(targetId).value = result;
}

const sls = document.querySelectorAll("[mtd-store-list]");


if (sls) {
    sls.forEach((list) => {

        const listId = list.getAttribute("mtd-store-list");
        const selectedId = document.getElementById(`${listId}-selected-id`).value;

        UpdateTarget(listId, selectedId);

        const inputNumber = document.getElementById(`${listId}-search-number`);
        const inputText = document.getElementById(`${listId}-search-text`);

        inputNumber.addEventListener('keydown', (e) => {
            if (e.keyCode == 13) {
                inputText.value = "";
                StoreListRequest(listId);
            }
        });

        inputText.addEventListener('keydown', (e) => {
            if (e.keyCode == 13) {
                inputNumber.value = "";
                StoreListRequest(listId);
            }
        });

        var selectRelated = new MTDSelectList(`${listId}-form-id`);
        selectRelated.selector.listen('MDCSelect:change', () => {
            StoreListRequest(listId);
        });

        // StoreListRequest(listId);

    });

}