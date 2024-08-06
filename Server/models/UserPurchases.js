const { DataTypes } = require("sequelize");
const sequelize = require("../config/database");

const UserPurchases = sequelize.define(
  "UserPurchases",
  {
    idx: {
      type: DataTypes.INTEGER,
      autoIncrement: true,
      primaryKey: true,
      allowNull: false,
    },
    user_uuid: {
      type: DataTypes.STRING(36),
      unique: true,
      allowNull: false,
    },
    rand_card_ids: {
      type: DataTypes.JSON,
      allowNull: false,
    },
  },
  {
    tableName: "user_purchases",
    timestamps: false,
  }
);

module.exports = UserPurchases;
