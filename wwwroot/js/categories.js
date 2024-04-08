const getCategories = async () => {
    let data = []
    await fetch('GetCategories', {
        method: "GET"
    })
        .then((response) => response.json())
        .then((json) => {
            data = json
        });
    return data;
}

const GetTable = (categories) => {
    return new DataTable('#categories', {
        responsive: true,
        data: categories,
        columns: [
            { data: 'name' },
            { data: 'films' },
            { data: 'nesting' },
            { defaultContent: '<button class="delete">Delete</button>' },
            { defaultContent: '<button class="update">Update</button>' }
        ]
    });
}

async function InitTable() {
    let categories = await getCategories();
    categories.forEach((item) => {
        if (item.films) {
            item.films = item.films.length;
        }
    })

    table = GetTable(categories)
    table.on('click', 'tbody tr td', function () {
        let data = table.row(this).data();
        if (this.firstChild.className === 'delete') {
            fetch(`DeleteCategory?id=${data.id}`, {
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