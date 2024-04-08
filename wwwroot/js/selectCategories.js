class SelectCategories{
    constructor(selectId, elements) {
        this.selectEl = document.getElementById(selectId);
        this.elements = elements;
        this.selectedOptions = [];
    }

    addSelectedOption = (innerText, value) => {
        let selectedEl = document.createElement("option");
        selectedEl.classList = "border m1 rounded";
        selectedEl.selected = true;
        selectedEl.innerText = innerText;
        selectedEl.value = value;
        this.selectedOptionsEl.appendChild(selectedEl);
    }

    addSelectOnChange = () => {
        [...this.selectEl.children].filter(option => !option.className.includes("selected-options")).forEach(option => {
            option.addEventListener("click", () => {
                if (this.selectedOptions && this.selectedOptions.some(op => op.value === option.dataset.value)) {
                    const element = [...this.selectedOptionsEl.childNodes].find(el => el.value === option.dataset.value);
                    this.selectedOptions = this.selectedOptions.filter(op => op.value !== option.dataset.value);
                    element.remove();
                }
                else {
                    this.selectedOptions.push({ text: option.innerText, value: option.dataset.value });

                    this.addSelectedOption(option.innerText, option.dataset.value);
                }
            });
        });

    }

    drawSelect = (selectedCategories) => {
        this.selectedOptionsEl = document.createElement("select");
        this.selectedOptionsEl.classList = "selected-options border d-ruby rounded";
        this.selectedOptionsEl.multiple = true;
        this.selectedOptionsEl.name = "CategoriesIds";
        this.selectEl.appendChild(this.selectedOptionsEl);

        this.elements.forEach(el => {
            let option = document.createElement("div");
            option.innerText = el.text;
            option.dataset.value = el.value;
            this.selectEl.appendChild(option);
            if (selectedCategories && selectedCategories.some(sel => sel === Number.parseInt(el.value))) {
                this.addSelectedOption(el.text, el.value)
                this.selectedOptions.push({ text: el.text, value: el.value });
            }
        });

        this.addSelectOnChange();
    }
}

const dataset = document.getElementById("film-categories").dataset;
const categories = JSON.parse(dataset.base).map((el) => { return { text: el.Text, value: el.Value } });

var cs = new SelectCategories("film-categories", categories)
if (dataset.selected) {
    const selectedCategories = JSON.parse(dataset.selected);
    cs.drawSelect(selectedCategories)
}
else {
    cs.drawSelect()
}