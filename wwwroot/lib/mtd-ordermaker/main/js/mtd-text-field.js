class MTDTextField {

    constructor(id) {
        this.base = document.getElementById(id);
        this.textField = new mdc.textField.MDCTextField(this.base);
        this.input = document.getElementById(`${id}-input`)
    }
}