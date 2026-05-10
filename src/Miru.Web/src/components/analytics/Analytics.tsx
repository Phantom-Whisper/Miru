type AnalyticsProps = {
    moviePercent: number;
    seriesPercent: number;
    totalHours: number;
    totalMovies?: number;
    totalSeries?: number;
};

function Analytics({
                       moviePercent,
                       seriesPercent,
                       totalHours,
                       totalMovies = 0,
                       totalSeries = 0,
                   }: AnalyticsProps) {

    const totalPercent = moviePercent + seriesPercent;

    const gradient = `conic-gradient(
        #8B5CF6 0% ${seriesPercent}%,
        #22D3EE ${seriesPercent}% ${totalPercent}%,
        rgba(255,255,255,0.08) ${totalPercent}% 100%
    )`;

    return (
        <div className="mt-6 rounded-2xl border border-white/5 bg-linear-to-br from-violet-500/10 to-cyan-500/5 p-3">

            <div className="flex items-center justify-between mb-3">
                <div>
                    <h3 className="text-sm font-semibold">
                        Stats
                    </h3>

                    <p className="text-[10px] text-zinc-400">
                        Watch time
                    </p>
                </div>

                <div className="relative w-12 h-12 shrink-0">

                    <div
                        className="absolute inset-0 rounded-full"
                        style={{
                            background: gradient,
                        }}
                    />

                    <div className="absolute inset-1.25 bg-zinc-900 rounded-full flex items-center justify-center">
                        <span className="text-[10px] font-bold">
                            {moviePercent + seriesPercent}%
                        </span>
                    </div>
                </div>
            </div>

            <div className="space-y-1 text-[11px] text-zinc-300">

                <div className="flex justify-between">
                    <span>
                        {totalSeries} series
                    </span>

                    <span className="text-violet-400">
                        {seriesPercent}%
                    </span>
                </div>

                <div className="flex justify-between">
                    <span>
                        {totalMovies} movies
                    </span>

                    <span className="text-cyan-400">
                        {moviePercent}%
                    </span>
                </div>

                <div className="flex justify-between">
                    <span>
                        {totalHours}h watched
                    </span>

                    <span className="text-zinc-500">
                        total
                    </span>
                </div>
            </div>
        </div>
    );
}

export default Analytics;