//$(document).ready(function () {
//    const $searchInput = $("#searchInput");
//    const $moviesContainer = $("#movies");
//    const $pagination = $("#pagination");
//    let currentPage = 1;
//    let totalPages = 1; // Initialize totalPages to 1

//    const itemsPerPage = 10; // Number of items per page

//    $searchInput.on("input", function () {
//        const searchTerm = $searchInput.val();

//        if (searchTerm.length >= 2) {
//            // Call your API to fetch movie results based on searchTerm
//            $.get(`/api/Movies/ReadFromElasticsearch?searchTerm=${searchTerm}`, function (data) {
//                displayMovieResults(data);
//            });
//        } else {
//            $moviesContainer.empty();
//            $pagination.empty();
//        }
//    });

//    function fetchMovieResults() {
//        const searchTerm = $searchInput.val();
//        $.get(`/api/Movies/ReadFromElasticsearch?searchTerm=${searchTerm}&page=${currentPage}`, function (data) {
//            displayMovieResults(data);
//        });
//    }

//    function displayMovieResults(data) {
//        $moviesContainer.empty();

//        data.results.forEach(function (movie) {
//            const $movieCard = $("<div>").addClass("movie");
//            $movieCard.append($("<img>").attr("src", movie.image_url));

//            const $movieInfo = $("<div>").addClass("movie-info");
//            $movieInfo.append($("<h3>").text(movie.name));
//            $movieInfo.append($("<span>").addClass("year").text(`Year: ${movie.year}`));

//            $movieCard.append($movieInfo);
//            $moviesContainer.append($movieCard);
//        });

//        totalPages = data.totalPages; // Update totalPages from the API response
//        displayPagination(totalPages);
//    }

//    function displayPagination(totalPages) {
//        $pagination.empty();

//        for (let i = 1; i <= totalPages; i++) {
//            const $pageLink = $("<a>").attr("href", "#").text(i).data("page", i);

//            if (i === currentPage) {
//                $pageLink.addClass("active");
//            }

//            $pageLink.on("click", function (e) {
//                e.preventDefault();
//                currentPage = $(this).data("page");
//                fetchMovieResults();
//            });

//            $pagination.append($pageLink);
//        }
//    }

//    fetchMovieResults();
//});
$(document).ready(function () {
    const $searchInput = $("#searchInput");
    const $moviesContainer = $("#movies");
    const $pagination = $("#pagination");
    let currentPage = 1;
    let totalPages = 1; // Initialize totalPages to 1

    const itemsPerPage = 10; // Number of items per page

    $searchInput.on("input", function () {
        const searchTerm = $searchInput.val();

        if (searchTerm.length >= 2) {
            // Call your API to fetch movie results based on searchTerm
            $.get(`/api/Movies/ReadFromElasticsearch?searchTerm=${searchTerm}`, function (data) {
                displayMovieResults(data);
            });
        } else {
            $moviesContainer.empty();
            $pagination.empty();
        }
    });

    function fetchMovieResults() {
        const searchTerm = $searchInput.val();
        $.get(`/api/Movies/ReadFromElasticsearch?searchTerm=${searchTerm}&page=${currentPage}`, function (data) {
            displayMovieResults(data);
        });
    }

    function displayMovieResults(data) {
        $moviesContainer.empty();

        data.results.forEach(function (movie) {
            const $movieCard = $("<div>").addClass("movie");
            $movieCard.append($("<img>").attr("src", "/images/DeffultImage.jpg").attr("alt", movie.name).addClass("card-img-top"));

            const $movieInfo = $("<div>").addClass("movie-info");
            $movieInfo.append($("<h6>").text(movie.name).addClass("card-text-center"));
            $movieInfo.append($("<span>").addClass("year").text(`Year: ${movie.year}`).addClass("card-year-bottom"));

            $movieCard.append($movieInfo);
            $moviesContainer.append($movieCard);
        });

        totalPages = data.totalPages; // Update totalPages from the API response
        displayPagination(totalPages);
    }

    function displayPagination(totalPages) {
        $pagination.empty();

        const $paginationList = $("<ul>").addClass("pagination");

        const visiblePageCount = 5; // Number of visible page links
        const halfVisiblePageCount = Math.floor(visiblePageCount / 2);

        let startPage = Math.max(currentPage - halfVisiblePageCount, 1);
        let endPage = Math.min(startPage + visiblePageCount - 1, totalPages);

        if (endPage - startPage + 1 < visiblePageCount) {
            startPage = Math.max(endPage - visiblePageCount + 1, 1);
        }

        if (startPage > 1) {
            const $firstPageItem = $("<li>").addClass("page-item");
            const $firstPageLink = $("<a>").addClass("page-link").attr("href", "#").text("First").data("page", 1);

            $firstPageLink.on("click", function (e) {
                e.preventDefault();
                currentPage = $(this).data("page");
                fetchMovieResults();
            });

            $firstPageItem.append($firstPageLink);
            $paginationList.append($firstPageItem);
        }

        const $prevPageItem = $("<li>").addClass("page-item");
        const $prevPageLink = $("<a>").addClass("page-link").attr("href", "#").text("Previous").data("page", currentPage - 1);

        $prevPageLink.on("click", function (e) {
            e.preventDefault();
            currentPage = $(this).data("page");
            fetchMovieResults();
        });

        $prevPageItem.append($prevPageLink);
        $paginationList.append($prevPageItem);

        for (let i = startPage; i <= endPage; i++) {
            const $pageItem = $("<li>").addClass("page-item");
            const $pageLink = $("<a>").addClass("page-link").attr("href", "#").text(i).data("page", i);

            if (i === currentPage) {
                $pageItem.addClass("active");
            }

            $pageLink.on("click", function (e) {
                e.preventDefault();
                currentPage = $(this).data("page");
                fetchMovieResults();
            });

            $pageItem.append($pageLink);
            $paginationList.append($pageItem);
        }

        const $nextPageItem = $("<li>").addClass("page-item");
        const $nextPageLink = $("<a>").addClass("page-link").attr("href", "#").text("Next").data("page", currentPage + 1);

        $nextPageLink.on("click", function (e) {
            e.preventDefault();
            currentPage = $(this).data("page");
            fetchMovieResults();
        });

        $nextPageItem.append($nextPageLink);
        $paginationList.append($nextPageItem);

        if (endPage < totalPages) {
            const $lastPageItem = $("<li>").addClass("page-item");
            const $lastPageLink = $("<a>").addClass("page-link").attr("href", "#").text("Last").data("page", totalPages);

            $lastPageLink.on("click", function (e) {
                e.preventDefault();
                currentPage = $(this).data("page");
                fetchMovieResults();
            });

            $lastPageItem.append($lastPageLink);
            $paginationList.append($lastPageItem);
        }

        $pagination.append($paginationList);
    }


    fetchMovieResults();
});
