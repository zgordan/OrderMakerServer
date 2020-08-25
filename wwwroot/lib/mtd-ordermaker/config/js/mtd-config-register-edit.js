new MTDTextField("form-name");
new MTDTextField("form-note");
new MTDSelectList("select-form");
new MTDSelectList("select-field");

const ChangeRegisterAction = (target) => {

    let id;
    if (target.id.includes('income')) {
        id = target.id.replace('-income', '-expense');        
    } else {
        id = target.id.replace('-expense', '-income');        
    }

    document.getElementById(id).checked = false;
}

const ChangeRegisterLinked = (target) => {

    const id = target.id.replace('-linked', '');

    if (!target.checked) {
        
        document.getElementById(`${id}-income`).checked = false;
        document.getElementById(`${id}-expense`).checked = false;

    } else {
        document.getElementById(`${id}-income`).checked = true;
    }
}