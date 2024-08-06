const User = require("./User");
const UserPurchases = require("./UserPurchases");
const Stage = require("./Stage");
const Card = require("./Card");
const Deck = require("./Deck");

User.hasOne(UserPurchases, {
  foreignKey: "user_uuid",
  sourceKey: "uuid",
  as: "user_purchases",
});

UserPurchases.belongsTo(User, {
  foreignKey: "user_uuid",
  targetKey: "uuid",
  as: "user",
});

User.hasOne(Stage, {
  foreignKey: "user_uuid",
  sourceKey: "uuid",
  as: "stage",
});

Stage.belongsTo(User, {
  foreignKey: "user_uuid",
  targetKey: "uuid",
  as: "user",
});

User.hasMany(Card, {
  foreignKey: "user_uuid",
  sourceKey: "uuid",
  as: "cards",
});

Card.belongsTo(User, {
  foreignKey: "user_uuid",
  targetKey: "uuid",
  as: "user",
});

User.hasOne(Deck, {
  foreignKey: "user_uuid",
  sourceKey: "uuid",
  as: "deck",
});

Deck.belongsTo(User, {
  foreignKey: "user_uuid",
  targetKey: "uuid",
  as: "user",
});
