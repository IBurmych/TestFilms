const getFilms = async () => {
    let data = []
     await fetch('GetFilms', {
        method: "GET"
    })
        .then((response) => response.json())
            .then((json) => {
                data = json
            });
    return data;
}

const GetTable = (films) => {
    return new DataTable('#films', {
        responsive: true,
        data: films,
        columns: [
            { data: 'name' },
            { data: 'release' },
            { data: 'director' },
            { data: 'categories' },
            { defaultContent: '<button class="delete">Delete</button>' },
            { defaultContent: '<button class="update">Update</button>' }
        ]
    });
}

const InitTable = async () => {
    let films = await getFilms();
    films.forEach((item) => {
        if (item.categories) {
            item.categories = item.categories.map(cat => cat.name).join(', ');
        }
    })
    table = GetTable(films)

    table.on('click', 'tbody tr td', function () {
        let data = table.row(this).data();
        if (this.firstChild.className === 'delete') {
            fetch(`DeleteFilm?id=${data.id}`, {
                    method: "DELETE"
            })
                .then(() => {
                    table.row(this).remove().draw();
                })
        }
        else if (this.firstChild.className === 'update') {
            window.location.href = `Update/${data.id}`
        }

    });
}
InitTable()
