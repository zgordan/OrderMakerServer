class MTDSelectList {

    constructor(id) {

        this.div = document.getElementById(id);
        this.selector = new mdc.select.MDCSelect(document.getElementById(id));
        this.input = document.getElementById(`${id}-input`);
        this.input.value = this.selector.value;

        this.selector.listen('MDCSelect:change', () => {            
            this.input.value = this.selector.value;  
        });

        this.itemTemplate = document.getElementById("mtd-select-list-helper-item");
    }

    AddItem(id, name, selected = false) {

        const li = this.itemTemplate.querySelector("li");
                
        let clone = li.cloneNode(true);              
        clone.setAttribute("data-value", id);
        const ripple = new mdc.ripple.MDCRipple(clone);            
        let text = clone.querySelector(".mdc-list-item__text");
        text.textContent = name;        
        const ul = this.div.querySelector(".mdc-list");
        ul.appendChild(clone);

        if (selected) {
            const index = ul.getElementsByTagName("li").length;
            this.selector.selectedIndex = index - 1;
        }
                      
    }

    RemoveItems() {
        this.selector.selectedIndex = -1;  
        const ul = this.div.querySelector(".mdc-list");
        ul.innerHTML = "";
        while (ul.firstChild) {
            ul.removeChild(ul.firstChild);
        }

    }
}