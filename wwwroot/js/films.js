const InitTable = (films) => {
    return new DataTable('#films', {
        responsive: true,
        data: films,
        columns: [
            { data: 'name' },
            { data: 'release' },
            { data: 'director' },
            { data: 'categories' }
        ]
    });
}

InitTable()
