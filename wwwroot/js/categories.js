const getCategories = async () => {
    let data = []
    await fetch('GetCategories', {
        method: "GET"
    }).then((response) => response.json())
        .then((json) => {
            data = json
        });
    return data;
}

const InitTable = () => {
    return new DataTable('#categories', {
        responsive: true,
        columns: [
            { data: 'id', visible: false},
            { data: 'name' },
            { data: 'films' },
            { data: 'nesting' }
        ]
    });
}
InitTable()