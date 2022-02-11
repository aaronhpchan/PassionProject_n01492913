namespace PassionProject_n01492913.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class wishlistsproducts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Wishlists",
                c => new
                    {
                        WishlistID = c.Int(nullable: false, identity: true),
                        WishlistName = c.String(),
                    })
                .PrimaryKey(t => t.WishlistID);
            
            CreateTable(
                "dbo.WishlistProducts",
                c => new
                    {
                        Wishlist_WishlistID = c.Int(nullable: false),
                        Product_ProductID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Wishlist_WishlistID, t.Product_ProductID })
                .ForeignKey("dbo.Wishlists", t => t.Wishlist_WishlistID, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.Product_ProductID, cascadeDelete: true)
                .Index(t => t.Wishlist_WishlistID)
                .Index(t => t.Product_ProductID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WishlistProducts", "Product_ProductID", "dbo.Products");
            DropForeignKey("dbo.WishlistProducts", "Wishlist_WishlistID", "dbo.Wishlists");
            DropIndex("dbo.WishlistProducts", new[] { "Product_ProductID" });
            DropIndex("dbo.WishlistProducts", new[] { "Wishlist_WishlistID" });
            DropTable("dbo.WishlistProducts");
            DropTable("dbo.Wishlists");
        }
    }
}
