// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/* ==========================
   TATTOO MODAL
========================== */

const tattooModal = document.getElementById('tattooModal');

if (tattooModal) {

    tattooModal.addEventListener('show.bs.modal', function (event) {

        const item = event.relatedTarget;

        if (!item) {
            return;
        }

        const modalTitle =
            document.getElementById('modalTitle');

        const modalCategory =
            document.getElementById('modalCategory');

        const modalDescription =
            document.getElementById('modalDescription');

        const modalImage =
            document.getElementById('modalImage');


        modalTitle.textContent =
            item.dataset.title || '';

        modalCategory.textContent =
            item.dataset.category || '';

        modalDescription.textContent =
            item.dataset.description || '';

        modalImage.src =
            item.dataset.image || '';

        modalImage.alt =
            item.dataset.title || '';

    });

}


/* ==========================
   GALLERY
========================== */

document.addEventListener("DOMContentLoaded", function () {

    const gallerySection =
        document.getElementById("gallery-section");

    if (!gallerySection) {
        return;
    }

    const galleryContent =
        document.getElementById("gallery-content");

    let isLoading = false;

    function updateActiveCategory(category) {

        const filters =
            gallerySection.querySelectorAll(".gallery-filter");

        filters.forEach(filter => {

            const filterCategory =
                filter.dataset.category || "";

            filter.classList.toggle(
                "active",
                filterCategory === category
            );

        });
    }

    async function loadGallery(
        category,
        page,
        updateHistory = true) {

        if (isLoading) {
            return;
        }

        isLoading = true;

        galleryContent.classList.add("is-loading");


        const params =
            new URLSearchParams();


        if (category) {
            params.set("category", category);
        }

        params.set("page", page);


        const url =
            `/Gallery/Content?${params.toString()}`;


        try {

            const response =
                await fetch(url);


            if (!response.ok) {
                throw new Error(
                    "Failed to load gallery."
                );
            }


            const html =
                await response.text();


            /*
             * Allow the existing gallery to
             * fade out before replacing it.
             */

            await new Promise(resolve =>
                setTimeout(resolve, 250)
            );


            galleryContent.innerHTML = html;
            updateActiveCategory(category);


            /*
             * Allow the browser to render the
             * new elements before fading them in.
             */

            requestAnimationFrame(() => {

                galleryContent.classList.remove(
                    "is-loading"
                );

            });


            /*
             * Update browser URL/history.
             */

            if (updateHistory) {

                const newUrl = category
                    ? `/Gallery?category=${encodeURIComponent(category)}&page=${page}`
                    : `/Gallery?page=${page}`;


                window.history.pushState(
                    {
                        category: category,
                        page: page
                    },
                    "",
                    newUrl
                );

            }


            /*
             * Scroll back to the top of the
             * gallery after changing category/page.
             */

            gallerySection.scrollIntoView({
                behavior: "smooth",
                block: "start"
            });

        }
        catch (error) {

            console.error(error);

            galleryContent.classList.remove(
                "is-loading"
            );

        }
        finally {

            isLoading = false;

        }

    }


    /* ==========================
       CATEGORY + PAGINATION
    ========================== */

    gallerySection.addEventListener(
        "click",
        function (event) {


            /*
             * Category filter
             */

            const categoryLink =
                event.target.closest(
                    ".gallery-filter"
                );


            if (categoryLink) {

                event.preventDefault();


                const category =
                    categoryLink.dataset.category || "";


                /*
                 * Changing category always
                 * starts at page 1.
                 */

                loadGallery(
                    category,
                    1
                );


                return;

            }


            /*
             * Pagination
             */

            const pageLink =
                event.target.closest(
                    ".gallery-page-link"
                );


            if (pageLink) {

                event.preventDefault();


                const page =
                    parseInt(
                        pageLink.dataset.page,
                        10
                    );


                const currentCategory =
                    new URLSearchParams(
                        window.location.search
                    ).get("category") || "";


                loadGallery(
                    currentCategory,
                    page
                );

            }

        }
    );


    /* ==========================
       BROWSER BACK / FORWARD
    ========================== */

    window.addEventListener(
        "popstate",
        function () {

            const params =
                new URLSearchParams(
                    window.location.search
                );


            const category =
                params.get("category") || "";


            const page =
                parseInt(
                    params.get("page"),
                    10
                ) || 1;


            loadGallery(
                category,
                page,
                false
            );

        }
    );

});