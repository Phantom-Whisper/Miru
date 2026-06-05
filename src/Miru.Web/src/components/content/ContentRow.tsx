import MediaCard from "../mediacard/MediaCard.tsx";
import SkeletonCard from "./SkeletonCard.tsx";
import {useEffect, useRef, useState} from "react";
import {ChevronLeft, ChevronRight} from "lucide-react";

type ContentRowProps = {
    title: string;
}

const movies = [
    {
        title: "Interstellar",
        image: "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
        progress: 72
    },
    {
        title: "Dune",
        image: "https://image.tmdb.org/t/p/w500/d5NXSklXo0qyIYkgV94XAgMIckC.jpg",
        progress: 34
    },
    {
        title: "Blade Runner 2049",
        image: "https://image.tmdb.org/t/p/w500/gajva2L0rPYkEWjzgFlBXCAVBE5.jpg",
        progress: 90
    },
    {
        title: "Interstellar",
        image: "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
        progress: 72
    },
    {
        title: "Dune",
        image: "https://image.tmdb.org/t/p/w500/d5NXSklXo0qyIYkgV94XAgMIckC.jpg",
        progress: 34
    },
    {
        title: "Blade Runner 2049",
        image: "https://image.tmdb.org/t/p/w500/gajva2L0rPYkEWjzgFlBXCAVBE5.jpg",
        progress: 90
    },
    {
        title: "Interstellar",
        image: "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
        progress: 72
    },
    {
        title: "Dune",
        image: "https://image.tmdb.org/t/p/w500/d5NXSklXo0qyIYkgV94XAgMIckC.jpg",
        progress: 34
    },
    {
        title: "Blade Runner 2049",
        image: "https://image.tmdb.org/t/p/w500/gajva2L0rPYkEWjzgFlBXCAVBE5.jpg",
        progress: 90
    },
];

function ContentRow({ title }: ContentRowProps) {
    const loading = false;
    const rowRef = useRef<HTMLDivElement>(null);

    const [canScrollLeft, setCanScrollLeft] = useState(false);
    const [canScrollRight, setCanScrollRight] = useState(true);

    const updateScrollButtons = () => {
        const row = rowRef.current;

        if (!row) return;

        setCanScrollLeft(row.scrollLeft > 0);

        setCanScrollRight(
            row.scrollLeft <
            row.scrollWidth - row.clientWidth - 10
        );
    };

    const scrollLeft = () => {
        rowRef.current?.scrollBy({
            left: -1000,
            behavior: "smooth"
        });
    };

    const scrollRight = () => {
        rowRef.current?.scrollBy({
            left: 1000,
            behavior: "smooth"
        });
    };

    useEffect(() => {
        updateScrollButtons();

        const row = rowRef.current;

        if (!row) return;

        row.addEventListener("scroll", updateScrollButtons);
        window.addEventListener("resize", updateScrollButtons);

        return () => {
            row.removeEventListener("scroll", updateScrollButtons);
            window.removeEventListener("resize", updateScrollButtons);
        };
    }, []);

    return (
        <section className="space-y-5">
            <div className="flex items-center justify-between">
                <h3 className="text-xl font-semibold">
                    {title}
                </h3>
            </div>

            <div className="relative group">
                <div className="absolute left-0 top-0 bottom-0 w-10 bg-linear-to-r from-black to-transparent z-10 pointer-events-none" />

                <div className="absolute right-0 top-0 bottom-0 w-10 bg-linear-to-l from-black to-transparent z-10 pointer-events-none" />

                <div className="relative group">
                    {canScrollLeft && (
                        <button
                            onClick={scrollLeft}
                            className="
                            absolute
                            left-2
                            top-1/2
                            -translate-y-1/2
                            z-20
                            h-12
                            w-12
                            rounded-full
                            bg-black/70
                            backdrop-blur
                            opacity-0
                            group-hover:opacity-100
                            transition
                            flex
                            items-center
                            justify-center
                        "
                        >
                            <ChevronLeft size={22} className="cursor-pointer" />
                        </button>
                    )}

                    {/* Right Button */}
                    {canScrollRight && (
                        <button
                            onClick={scrollRight}
                            className="
                            absolute
                            right-2
                            top-1/2
                            -translate-y-1/2
                            z-20
                            h-12
                            w-12
                            rounded-full
                            bg-black/70
                            backdrop-blur
                            opacity-0
                            group-hover:opacity-100
                            transition
                            flex
                            items-center
                            justify-center
                        "
                        >
                            <ChevronRight size={22} className="cursor-pointer" />
                        </button>
                    )}

                    <div
                        ref={rowRef}
                        className="flex gap-5 overflow-x-auto scrollbar-hide py-4">
                        {loading
                            ? Array.from({ length: 6 }).map((_, i) => (
                                <SkeletonCard key={i} />
                            ))
                            : movies.map((movie) => (
                                <div
                                    key={movie.title}
                                    className="transition-all duration-300 group-hover:opacity-40 hover:opacity-100!"
                                >
                                    <MediaCard {...movie} />
                                </div>
                            ))
                        }
                    </div>
                </div>
            </div>
        </section>
    )
}

export default ContentRow