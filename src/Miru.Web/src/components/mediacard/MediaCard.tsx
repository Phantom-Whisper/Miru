type MediaCardProps = {
    title: string;
    // description: string;
    image: string;
    progress?: number;
}

function MediaCard({
    title,
    // description,
    image,
    progress = 0} : MediaCardProps
    ){
    return (
        <div className="group relative min-w-45 h-65 rounded-2xl overflow-hidden shrink-0 cursor-pointer transition-all duration-300 hover:scale-110 hover:z-20">
            <img
                src={image}
                alt={title}
                className="w-full h-full object-cover"
            />

            <div className="absolute inset-0 bg-linear-to-t from-black/90 via-black/20 to-transparent opacity-80" />

            <div className="absolute bottom-0 left-0 right-0 p-3">
                <h4 className="font-semibold text-sm mb-2">
                    {title}
                </h4>

                {progress > 0 && (
                    <div className="w-full h-1 bg-zinc-700 rounded-full overflow-hidden">
                        <div
                            className="h-full bg-red-500 rounded-full"
                            style={{ width: `${progress}%` }}
                        />
                    </div>
                )}
            </div>
        </div>
    )
}

export default MediaCard;