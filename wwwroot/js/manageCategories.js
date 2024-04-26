class ManageCategories {
    constructor(selectId, surveyContainerUpdateId, surveyContainerAddId) {
        this.baseUrl = `${window.location.origin}/CategoryApi`;
        this.getCategories();
        this.surveyContainerUpdateId = surveyContainerUpdateId;
        this.surveyContainerAddId = surveyContainerAddId;
        this.selectEl = document.getElementById(selectId);
    }

    drowCategorySelectOptions = () => {
        this.categories.forEach(cat => {
            let optionEl = document.createElement("option");
            optionEl.classList = "border m1 rounded";
            optionEl.innerText = cat.name;
            optionEl.value = cat.id;
            this.selectEl.appendChild(optionEl);

            this.selectEl.addEventListener("change", () => {
                this.selectedCat = this.categories.find(el => el.id == this.selectEl.value);
                this.drowUpdateCategory();
            });
        });
    }

    drowAddCategory = () => {
        const surveyJson = {
            checkErrorsMode: "onComplete",
            validationEnabled: true,
            isValidatingOnServer: true,
            elements: [{
                name: "name",
                isRequired: true,
                title: "Enter category name",
                type: "text"
            }, {
                type: "dropdown",
                name: "parentCategoryId",
                title: "Select parent category",
                choices: this.categories.map(cat => { return { value: cat.id, text: cat.name } })
            }]
        };
        const surveyAdd = new Survey.Model(surveyJson);
        surveyAdd.onComplete.add(async (sender, options) => {
            await this.addCategory({
                name: sender.data.name,
                parentCategoryId: sender.data.parentCategoryId
            });
        });
        $(() => {
            $(`#${this.surveyContainerAddId}`).Survey({ model: surveyAdd });
        });
    }

    drowUpdateCategory = () => {
        
        const surveyJson = {
            checkErrorsMode: "onComplete",
            validationEnabled: true,
            isValidatingOnServer: true,
            elements: [{
                name: "id",
                isRequired: true,
                defaultValue: this.selectedCat.id,
                type: "text",
                visibleIf:"false"
            }, {
                name: "name",
                isRequired: true,
                title: "Enter category name",
                defaultValue: this.selectedCat.name,
                type: "text"
            }, {
                type: "dropdown",
                name: "parentCategoryId",
                title: "Select parent category",
                defaultValue: this.selectedCat.parentCategory?.id ?? 0,
                choices: this.categories.map(cat => { return { value: cat.id, text: cat.name } })
            }]
        };
        const surveyUpdate = new Survey.Model(surveyJson);
        surveyUpdate.onServerValidateQuestions.add(this.validateUpdateDomain);
        $(() => {
            $(`#${this.surveyContainerUpdateId}`).Survey({ model: surveyUpdate });
            this.drowDeleteButton();
        });

    }

    drowDeleteButton = () => {
        let btn = document.createElement("div");
        btn.classList = "sd-btn sd-btn--action sd-navigation__complete-btn";
        btn.innerText = "Delete";
        btn.addEventListener("click", async () => {
            await this.deleteCategory();
        });
        document.getElementById(this.surveyContainerUpdateId).appendChild(btn);
    }

    validateUpdateDomain = async (sender, options) => {
        let isOk = await this.updateCategory({
            id: sender.jsonObj.elements.find(el => el.name === "id").defaultValue,
            name: sender.data.name,
            parentCategoryId: sender.data.parentCategoryId
        });
        isOk ? options.complete() : options.error = "LOOP";
    }

    getCategories = async () => {
        let data = []
        await fetch(`${this.baseUrl}/GetAll`, {
            method: "GET"
        }).then((response) => response.json())
            .then((json) => {
                data = json;
            });
        this.categories = data;
        this.drowCategorySelectOptions();
        this.drowAddCategory();
    }

    addCategory = async (categoryFormModel) => {
        await fetch(`${this.baseUrl}/Create`, {
            method: "POST",
            body: JSON.stringify(categoryFormModel),
            headers: {
                "Content-Type": "application/json",
            },
        });
    }

    updateCategory = async (categoryFormModel) => {
        let category = this.categories.find(el => el.id == categoryFormModel.id);
        category.name = categoryFormModel.name;
        category.parentCategoryId = categoryFormModel.parentCategoryId;
        let isOk;
        await fetch(`${this.baseUrl}/Update`, {
            method: "PATCH",
            body: JSON.stringify(category),
            headers: {
                "Content-Type": "application/json",
            },
        }).then((response) => {
            isOk = response.ok;
        })
        return isOk;
    }

    deleteCategory = async () => {
        await fetch(`${this.baseUrl}/Delete/?id=${this.selectedCat.id}`, {
            method: "DELETE",
        });
        location.reload();
    }
}

const manageCategories = new ManageCategories("CategorySelect", "surveyContainerUpdate", "surveyContainerAdd");
