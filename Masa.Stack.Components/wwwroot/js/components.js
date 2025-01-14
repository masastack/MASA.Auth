window.MasaStackComponents = {}

window.MasaStackComponents.scrollTo = (target, inside = 'window') => {
    const targetElement = document.querySelector(target)
    if (!targetElement) return;

    const targetRect = targetElement.getBoundingClientRect();

    if (inside === 'window') {
        const scrollTop = document.documentElement.scrollTop;

        let top = targetRect.top + scrollTop;

        window.scrollTo({top, left: 0, behavior: "smooth"});
    } else {
        const insideElement = document.querySelector(inside);
        if (!insideElement) return;

        const scrollTop = insideElement.scrollTop;
        const insideRect = insideElement.getBoundingClientRect();

        let top = targetRect.top + scrollTop - insideRect.top;

        insideElement.scrollTo({ top, left: 0, behavior: "smooth" });
        const allTitleCategories = document.querySelectorAll('.category_title_app > .title_category')
        allTitleCategories.forEach((element) => {
            if (targetElement != element) {
                element.classList.remove('title_category_active')
            } else {
                if (!element.classList.contains('title_category_active')) {
                    element.classList.add('title_category_active')
                }
            }
        }); 

        var sideId = target +"_side";
        const sideElement = document.querySelector(sideId);
        sideElement.parentNode.childNodes.forEach((element) => {
            if (element.nodeType != 1) {
                return;
            }
            if (sideElement != element) {
                element.classList.remove('category_side_active')
            } else {
                if (!element.classList.contains('category_side_active')) {
                    element.classList.add('category_side_active')
                }
            }
        });

    }
}

window.MasaStackComponents.waterFull = (containerSelector, selectors, columns = 4) => {
    const container = document.querySelector(containerSelector);
    const items = document.querySelectorAll(`${containerSelector} ${selectors}`);
    const arr = []
    let width = 0;
    for (let index = 0; index < items.length; index++) {
        const item = items[index];

        if (width === 0) width = item.clientWidth;

        if (index < columns) {
            item.style['position'] = 'absolute';
            item.style['top'] = "0px";
            const left = width * index;
            item.style['left'] = `${left}px`;
            arr.push({height: item.clientHeight, left})
        } else {
            arr.sort((x, y) => x.height - y.height);
            const res = arr[0];
            item.style['position'] = 'absolute';
            item.style['top'] = `${res.height}px`;
            item.style['left'] = `${res.left}px`;
            res.height = res.height + item.clientHeight;
        }
    }

    arr.sort((x, y) => x.height - y.height)

    const maxHeight = arr[arr.length - 1];
    return maxHeight.height;
}

window.MasaStackComponents.listenScroll = (selector, childSelectors, dotNet) => {
    let el = document.querySelector(selector);
    if (!el) return;

    const children = document.querySelectorAll(`${selector} ${childSelectors}`);

    let fn = debounce((position) => {
        const elTop = el.offsetTop;

        const childrenTops = []
        for (const child of children) {
            childrenTops.push(child.offsetTop)
        }

        const computedChildrenTops = childrenTops.map(child => child - elTop - 8);

        let index = computedChildrenTops.findIndex(child => child >= position);

        if (index === -1) {
            index = computedChildrenTops.length - 1;
        } else if (index > 0) {
            index--;
        }
    }, 100)

    el.addEventListener("scroll", function (e) {
        fn(e.target.scrollTop)
    })
}

window.MasaStackComponents.resizeObserver = (selector, invoker) => {
    const resizeObserver = new ResizeObserver((entries => {
        invoker.invokeMethodAsync('Invoke');
    }));
    var target = document.querySelector(selector);
    if (target)
    {
        resizeObserver.observe(target);
    }  
}

window.MasaStackComponents.intersectionObserver = (selector, invoker) => {
    const observer = new IntersectionObserver((entries) => {
        if (entries.some(e => e.isIntersecting)) {
            invoker.invokeMethodAsync('Invoke')
        }
    })

    observer.observe(document.querySelector(selector))
}

window.MasaStackComponents.getTimezoneOffset = function() {
    return new Date().getTimezoneOffset();
}

let masonryInstances = {};

window.MasaStackComponents.initMasonry = (selector, itemSelector, gutter) => {
    if (!selector || !itemSelector) {
        console.error('Invalid selector or itemSelector provided.');
        return;
    }

    const elem = document.querySelector(selector);
    if (!elem) {
        console.warn(`Element not found for selector: ${selector}`);
        return;
    }

    if (typeof Masonry === 'undefined') {
        console.warn('Masonry is not available. Falling back to normal layout.');
        return;
    }

    if (masonryInstances[selector]) {
        masonryInstances[selector].destroy();
    }

    try {
        masonryInstances[selector] = new Masonry(elem, {
            itemSelector: itemSelector,
            columnWidth: itemSelector,
            percentPosition: true,
            gutter: gutter || 0
        });

        requestAnimationFrame(() => {
            masonryInstances[selector].layout();
        });
    } catch (error) {
        console.warn('Falling back to normal layout.');
    }
};

function debounce(fn, wait) {
    let timer = null;
    return function (...args) {
        if (timer) clearTimeout(timer);
        timer = setTimeout(() => {
            fn.apply(this, args)
        }, wait)
    }
}

function getDom(el) {
    if (typeof el === 'string') {
        return document.querySelector(el)
    }

    return el
}
