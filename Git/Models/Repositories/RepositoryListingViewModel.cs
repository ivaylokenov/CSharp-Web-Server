namespace Git.Models.Repositories
{
    public class RepositoryListingViewModel
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string Owner { get; init; }

        public string CreatedOn { get; init; }

        public int Commits { get; init; }
    }
}
