class ExctensionForm {
    constructor() {

        this.selectScript = new MTDSelectList("index-filter-extension-list");
    }
}


class CustomForm {
    constructor() {
        this.resultId = document.getElementById("index-filter-custom-result-id");
        this.resultType = document.getElementById("index-filter-custom-result-type");
        this.resultAction = document.getElementById("index-filter-custom-result-action");
        this.resultValue = document.getElementById("index-filter-custom-result-value");

        this.selectAction = new MTDSelectList("index-filter-custom-action");
        this.selectValue = new MTDSelectList("index-filter-custom-list-value");
        this.selectFields = new MTDSelectList("index-filter-custom-fields");

        this.textValue = new MTDTextField("index-filter-custom-text-value");
        this.intValue = new MTDTextField("index-filter-custom-int-value");
        this.boolValue = new MTDSelectList("index-filter-custom-bool-value");
        this.dateValue = new MTDTextField("index-filter-custom-date-value");
    }
}


const IndexFilterShowTab = (tabShow) => {

    const tabs = ['custom', 'service', 'extension'];
    tabs.forEach((tabName) => {
        const div = document.getElementById(`index-filter-${tabName}`);        
        div.style.display = tabName === tabShow ? "block" : "none";
    });
}

const ShowCustomFieldTypes = (fiedShow) => {

    const tabs = ['text', 'int', 'date', 'list', 'bool', 'text'];
    tabs.forEach((tabName) => {
        const div = document.getElementById(`index-filter-custom-${tabName}`);
        div.style.display = tabName === fiedShow ? "block" : "none";
    });
}

const Service = () => {

    new MTDTextField("index-filter-date-start");
    new MTDTextField("index-filter-date-finish");

    const period = document.getElementById("index-filter-service-period");
    const owners = document.getElementById("index-filter-service-owners");

    const select = new MTDSelectList("index-filter-service-list");
    const ownersList = new MTDSelectList("index-filter-service-owners-list");

    select.selector.listen('MDCSelect:change', () => {
        period.classList.toggle("mtd-main-display-none");
        owners.classList.toggle("mtd-main-display-none");
    });

}

const GetCustomPartForType = (dataType) => {
    let result;
    switch (dataType) {
        case "2":
        case "3": {
            result = "int";
            break;
        }
        case "5":
        case "6": {
            result = "date";
            break;
        }
        case "11": {
            result = "list";
            break;
        }
        case "12": {
            result = "bool";
            break;
        }
        default: {
            result = "text";
            break;
        }
    }

    return result;
}

const RequestFieldList = (idField, selectValue) => {

    const form = document.getElementById('index-filter-custom-list-form');
    const inputField = form.querySelector("input[id='id-field']");
    inputField.value = idField;
    const formData = CreateFormData(form);

    fetch(form.action, { method: form.method, body: formData })
        .then((response) => {
            return response.json();
        })
        .then((data) => {
            if (data.value) {

                selectValue.RemoveItems();
                data.value.forEach((item, index) => {
                    selected = false;
                    if (index === 0) { selected = true; }
                    selectValue.AddItem(item.id, item.value, selected);
                });
            }
        });
}

const Custom = () => {

    const cf = new CustomForm();

    const li = cf.selectFields.div.querySelector(`[data-value='${cf.selectFields.selector.value}']`);
    const fieldShow = GetCustomPartForType(li);
    ShowCustomFieldTypes(fieldShow);

    cf.selectFields.selector.listen('MDCSelect:change', () => {

        const li = cf.selectFields.div.querySelector(`[data-value='${cf.selectFields.selector.value}']`);
        if (!li) return;

        const dataType = li.getAttribute("data-type");
        const fieldShow = GetCustomPartForType(dataType);

        if (dataType === '11') { RequestFieldList(cf.selectFields.selector.value, cf.selectValue); }
        cf.selectAction.div.classList.remove("mtd-main-display-none");
        const separ = document.getElementById("custom-separ");
        separ.classList.remove("mtd-main-display-none");
        if (dataType === "12" || dataType === '11') { cf.selectAction.div.classList.add("mtd-main-display-none"); separ.classList.add("mtd-main-display-none"); }
        ShowCustomFieldTypes(fieldShow);

        cf.resultType.value = dataType;
        cf.resultId.value = cf.selectFields.selector.value;
        cf.textValue.textField.value = "";
        cf.intValue.textField.value = "";
        cf.boolValue.selector.value = "0";
        cf.dateValue.textField.value = "";

        cf.resultValue.value = "";

    });

    cf.selectAction.selector.listen('MDCSelect:change', () => {
        cf.resultAction.value = cf.selectAction.selector.value;
    });

    cf.selectValue.selector.listen('MDCSelect:change', () => {
        cf.resultValue.value = cf.selectValue.selector.value;
    });

    cf.boolValue.selector.listen('MDCSelect:change', () => {
        cf.resultValue.value = cf.boolValue.selector.value;
    });

    cf.textValue.input.addEventListener("input", () => {
        cf.resultValue.value = cf.textValue.textField.value;
    });
    //cf.intValue = new MTDTextField("index-filter-custom-int-value");
    //cf.boolValue = new MTDSelectList("index-filter-custom-bool-value");
    //cf.dateValue = new MTDTextField("index-filter-custom-date-value");

}

const Extension = () => {

    const ef = new ExctensionForm();
}

const WindowHandler = () => {

    const indexFilterWindow = document.getElementById("index-filter-window");
    const indexFilterButton = document.getElementById("index-filter-button");

    if (indexFilterButton) {
        indexFilterButton.addEventListener('click', () => {
            indexFilterWindow.style.display = "block";
            IndexShowModal(true, false);
        });
    }

    const tabBar = document.getElementById('index-filter-tabs');
    new mdc.tabBar.MDCTabBar(tabBar);
}



const IndexFilterClose = () => {
    const indexFilterWindow = document.getElementById("index-filter-window");
    indexFilterWindow.style.display = "none";
    IndexShowModal(false, false);
}

(() => {

    WindowHandler();
    Service();
    Custom();
    Extension();

})();