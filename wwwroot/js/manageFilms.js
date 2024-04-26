class ManageFilms {
    constructor(selectId, surveyContainerUpdateId, surveyContainerAddId) {
        this.baseUrl = `${window.location.origin}/FilmApi`;
        this.getFilms();
        this.surveyContainerUpdateId = surveyContainerUpdateId;
        this.surveyContainerAddId = surveyContainerAddId;
        this.selectEl = document.getElementById(selectId);
    }

    drowFilmSelectOptions = () => {
        this.films.forEach(f => {
            let optionEl = document.createElement("option");
            optionEl.classList = "border m1 rounded";
            optionEl.innerText = f.name;
            optionEl.value = f.id;
            this.selectEl.appendChild(optionEl);

            this.selectEl.addEventListener("change", () => {
                this.selectedFilm = this.films.find(el => el.id == this.selectEl.value);
                this.drowUpdateFilm();
            });
        });
    }

    drowAddFilm = () => {
        const surveyJson = {
            checkErrorsMode: "onComplete",
            validationEnabled: true,
            isValidatingOnServer: true,
            elements: [{
                name: "name",
                isRequired: true,
                title: "Enter film name",
                type: "text"
            }, {
                name: "director",
                isRequired: true,
                title: "Enter film director",
                type: "text"
            }, {
                name: "release",
                isRequired: true,
                title: "Enter film release",
                type: "text",
                inputType: "datetime",
                defaultValueExpression: "today()"
            }, {
                type: "tagbox",
                name: "categoriesIds",
                title: "Select categories",
                choices: this.categories.map(cat => { return { value: cat.id, text: cat.name } })
            }]
        };
        const surveyAdd = new Survey.Model(surveyJson);
        surveyAdd.onComplete.add(async (sender, options) => {
            await this.addFilm(sender.data);
        });
        $(() => {
            $(`#${this.surveyContainerAddId}`).Survey({ model: surveyAdd });
        });
    }

    drowUpdateFilm = () => {

        const surveyJson = {
            checkErrorsMode: "onComplete",
            validationEnabled: true,
            isValidatingOnServer: true,
            elements: [{
                name: "id",
                isRequired: true,
                defaultValue: this.selectedFilm.id,
                type: "text",
                visibleIf: "false"
            }, {
                name: "name",
                isRequired: true,
                title: "Enter Film name",
                defaultValue: this.selectedFilm.name,
                type: "text"
            }, {
                name: "director",
                isRequired: true,
                title: "Enter film director",
                defaultValue: this.selectedFilm.director,
                type: "text"
            }, {
                name: "release",
                isRequired: true,
                title: "Enter film release",
                defaultValue: this.selectedFilm.release,
                type: "text",
                inputType: "datetime",
            }, {
                type: "tagbox",
                name: "categoriesIds",
                title: "Select categories",
                defaultValue: this.selectedFilm.categories?.map(cat => cat.id) ?? 0,
                choices: this.categories.map(cat => { return { value: cat.id, text: cat.name } })
            }]
        };
        const surveyUpdate = new Survey.Model(surveyJson);
        surveyUpdate.onComplete.add(async (sender, options) => {
            let film = this.films.find(el => el.id == sender.jsonObj.elements.find(el => el.name === "id").defaultValue);
            film.name = sender.data.name;
            film.director = sender.data.director;
            film.release = sender.data.release;
            film.categoriesIds = sender.data.categoriesIds;
            await this.updateFilm(film);
        });
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
            await this.deleteFilm();
        });
        document.getElementById(this.surveyContainerUpdateId).appendChild(btn);
    }

    getFilms = async () => {
        let data = []
        await fetch(`${this.baseUrl}/GetAll`, {
            method: "GET"
        }).then((response) => response.json())
            .then((json) => {
                data = json;
            });
        this.films = data;
        this.drowFilmSelectOptions();
        await this.getCategories()
        this.drowAddFilm();
    }

    addFilm = async (filmFormModel) => {
        await fetch(`${this.baseUrl}/Create`, {
            method: "POST",
            body: JSON.stringify(filmFormModel),
            headers: {
                "Content-Type": "application/json",
            },
        });
    }

    updateFilm = async (film) => {
        film.categories = undefined;
        await fetch(`${this.baseUrl}/Update`, {
            method: "PATCH",
            body: JSON.stringify(film),
            headers: {
                "Content-Type": "application/json",
            },
        });
    }

    getCategories = async () => {
        let data = []
        await fetch(`${window.location.origin}/CategoryApi/GetAll`, {
            method: "GET"
        }).then((response) => response.json())
            .then((json) => {
                data = json;
            });
        this.categories = data;
    }

    deleteFilm = async () => {
        await fetch(`${this.baseUrl}/Delete/?id=${this.selectedFilm.id}`, {
            method: "DELETE",
        });
        location.reload();
    }
}

const manageFilms = new ManageFilms("FilmSelect", "surveyContainerUpdate", "surveyContainerAdd");
