/*
    MTD OrderMaker - http://ordermaker.org
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.

    This file is part of MTD OrderMaker.
    MTD OrderMaker is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see  https://www.gnu.org/licenses/.
*/

const snackbar = new mdc.snackbar.MDCSnackbar(document.getElementById('main-snack'));

const MainShowSnackBar = (message, error = false) => {
    const div = document.getElementById('main-snack');
    div.classList.add("mdc-snackbar--open");
    snackbar.labelText = message;
    if (error) {
        snackbar.timeoutMs = 10000;
    }
    snackbar.open();
}


const newGuid = () => {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

const rippleFor = (className) => {
    const ripples = document.querySelectorAll(className);
    ripples.forEach((obj) => {
        const ripple = new mdc.ripple.MDCRipple(obj);
        ripple.unbounded = true;
    });
    return true;
}

const ListenerForDataHref = () => {
    const items = document.querySelectorAll('[mtd-data-href]');
    items.forEach((item) => {
        item.addEventListener('click', () => {
            ActionShowModal();
            const href = item.getAttribute('mtd-data-href');
            const target = item.getAttribute('mtd_data_href-target');
            if (target) {
                window.open(href, target);
                ActionShowModal(false);
            } else {
                window.location.href = href;
            }
        });
    });
}

const CreateFormData = (form) => {

    var formData = new FormData();
    for (var i = 0; i < form.length; i++) {

        if (form[i].getAttribute("type") == 'checkbox') {
            formData.append(form[i].name, form[i].checked);
        } else {
            formData.append(form[i].name, form[i].value);
        }

        if (form[i].files && form[i].files.length > 0) {
            formData.append(form[i].name, form[i].files[0], form[i].files[0].name);
        }
    }

    return formData;
}

const CheckRequired = (form) => {
    let counter = 0;
    if (form) {
        const rs = form.querySelectorAll("[required]");
        if (rs) {
            rs.forEach((obj) => {
                if (!obj.value) { counter++; }
            });
        }
    }
    return counter;
}

const ListenerForPostData = () => {

    const forms = document.querySelectorAll("form[mtd-data-form]");
    forms.forEach((form) => {

        const result = form.querySelector('input[mtd-data-result]');
        const action = form.getAttribute('action');
        const clickers = form.querySelectorAll('[mtd-data-clicker]');
        const inputs = form.querySelectorAll('input[mtd-data-input]');

        clickers.forEach((clicker) => {
            const value = clicker.getAttribute('mtd-data-clicker');
            const location = clicker.getAttribute('mtd-data-location');
            const message = clicker.getAttribute('mtd-data-message');

            clicker.addEventListener('click', () => {

                const check = CheckRequired(form);
                if (check > 0) {
                    const dialog = new mdc.dialog.MDCDialog(document.getElementById('dialog-main-info'));
                    dialog.open();
                    return false;
                }

                ActionShowModal();
                if (value) { result.value = value; }

                const formData = CreateFormData(form);

                const xmlHttp = new XMLHttpRequest();
                xmlHttp.open("post", action, true);
                xmlHttp.send(formData);
                xmlHttp.onreadystatechange = function () {

                    if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                        //document.location.reload(location.hash, '');
                        if (message) { sessionStorage.setItem("Message", message); }

                        if (location) {
                            document.location = location;
                        } else {
                            document.location.reload();
                        }
                    }

                    if (xmlHttp.status == 400) {
                        setTimeout(() => { ActionShowModal(false); MainShowSnackBar(xmlHttp.responseText, true);}, 1000);                                                                        
                    }

                    if (xmlHttp.status == 500) {
                        setTimeout(() => { ActionShowModal(false); MainShowSnackBar("500 Internal Server Error", true); }, 1000);
                    }
                }
            });
        });

        inputs.forEach((input) => {
            const location = input.getAttribute('mtd-data-location');
            const message = input.getAttribute('mtd-data-message');
            input.addEventListener('keydown', (e) => {
                if (e.keyCode === 13) {

                    const check = CheckRequired();
                    if (check > 0) {
                        return false;
                    }

                    e.preventDefault();
                    ActionShowModal();
                    var xmlHttp = new XMLHttpRequest();
                    xmlHttp.open("POST", form.getAttribute("action"), true);

                    var formData = CreateFormData(form);

                    xmlHttp.send(formData);
                    xmlHttp.onreadystatechange = function () {
                        if (xmlHttp.readyState == 4 && xmlHttp.status == 200) {
                            //document.location.reload(location.hash, '');

                            if (message) { sessionStorage.setItem("Message", message); }

                            if (location) {
                                document.location = location;
                            } else {
                                document.location.reload();
                            }
                        }

                        if (xmlHttp.status == 400) {
                            setTimeout(() => { ActionShowModal(false); MainShowSnackBar(xmlHttp.responseText, true); }, 1000);  
                        }

                        if (xmlHttp.status == 500) {
                            setTimeout(() => { ActionShowModal(false); MainShowSnackBar("500 Internal Server Error", true); }, 1000);
                        }
                    }
                }
            });
        });

    });
}

const ActionShowModal = (show = true) => {
    const style = show == true ? 'block' : 'none';
    document.getElementById('mainModal').style.display = style;
}

const ListenerForAction = () => {

    const button = document.getElementById('main-action-button');
    const action = document.getElementById('main-action-menu');
    if (action) {
        const menu = new mdc.menu.MDCMenu(action);
        menu.setFixedPosition(true);
        button.addEventListener('click', () => { menu.open = true; })
    }
}

const DetectMobile = () => {

    if (navigator.userAgent.match(/Android/i)
        || navigator.userAgent.match(/webOS/i)
        || navigator.userAgent.match(/iPhone/i)
        || navigator.userAgent.match(/iPad/i)
        || navigator.userAgent.match(/iPod/i)
        || navigator.userAgent.match(/BlackBerry/i)
        || navigator.userAgent.match(/Windows Phone/i)
    ) {
        document.body.style.height = "";
    }
}


(() => {

    DetectMobile();

    if (sessionStorage.getItem("Message")) {

        MainShowSnackBar(sessionStorage.getItem("Message"));
        sessionStorage.removeItem("Message");
    }

    if (sessionStorage.getItem("ErrorMessage")) {

        MainShowSnackBar(sessionStorage.getItem("ErrorMessage"), true);
        sessionStorage.removeItem("ErrorMessage");
    }

    ListenerForAction();

    rippleFor('.mdc-checkbox');
    rippleFor('.mdc-icon-button');
    rippleFor('.mdc-list-item');
    rippleFor('.mdc-fab');
    rippleFor('.mdc-button');
    rippleFor('.mdc-card__primary-action');

    const textFields = document.querySelectorAll('.mdc-text-field');
    textFields.forEach((item) => {
        const textField = new mdc.textField.MDCTextField(item);
        const fond = new mdc.textField.MDCTextFieldFoundation(textField);
    });

    const toggleButtons = document.querySelectorAll('.mdc-icon-button');
    toggleButtons.forEach((item) => {
        new mdc.iconButton.MDCIconButtonToggle(item);
    });



    const drawer = mdc.drawer.MDCDrawer.attachTo(document.querySelector('.mdc-drawer'));

    const topAppBar = mdc.topAppBar.MDCTopAppBar.attachTo(document.getElementById('app-bar'));
    topAppBar.setScrollTarget(document.getElementById('main-content'));
    topAppBar.listen('MDCTopAppBar:nav', () => {
        drawer.open = !drawer.open;
    });

    let checked = 0;
    const url = window.location.href.toLowerCase();

    if (url.includes("/identity/users")) {
        checked++;
        document.getElementById("menu-users").classList.add("mdc-list-item--activated");
    }

    if (url.includes("/config")) {
        checked++;
        document.getElementById("menu-config").classList.add("mdc-list-item--activated");
    }

    if (url.includes("/identity/account/manage")) {
        checked++;
        document.getElementById("menu-account").classList.add("mdc-list-item--activated");
    }

    if (checked === 0) {
        document.getElementById("menu-desk").classList.add("mdc-list-item--activated");
    }


    ListenerForDataHref();
    ListenerForPostData();

    const mainUserButton = document.getElementById('main-user-button');
    if (mainUserButton) {
        const mainUserMenu = new mdc.menuSurface.MDCMenuSurface(document.getElementById('main-user-menu'));

        mainUserMenu.setFixedPosition(true);
        mainUserButton.addEventListener('click', () => {
            mainUserMenu.open = !mainUserMenu.open;
        });
    }

    const mainAppsButton = document.getElementById('main-apps-button');
    if (mainAppsButton) {
        const mainUserMenu = new mdc.menuSurface.MDCMenuSurface(document.getElementById('main-apps-menu'));

        mainUserMenu.setFixedPosition(true);
        mainAppsButton.addEventListener('click', () => {
            mainUserMenu.open = !mainUserMenu.open;
        });
    }

    const selectLists = document.querySelectorAll(".mdc-select");
    if (selectLists) {
        selectLists.forEach((item) => {
           const selector = new mdc.select.MDCSelect(item);
        });
    }
    

})();