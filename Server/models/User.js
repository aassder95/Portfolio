const { DataTypes } = require("sequelize");
const sequelize = require("../config/database");

const User = sequelize.define(
  "User",
  {
    idx: {
      type: DataTypes.INTEGER,
      autoIncrement: true,
      primaryKey: true,
      allowNull: false,
    },
    uuid: {
      type: DataTypes.STRING(36),
      unique: true,
      allowNull: false,
    },
    name: {
      type: DataTypes.STRING,
      allowNull: false,
    },
    gold: {
      type: DataTypes.INTEGER,
      defaultValue: 0,
      allowNull: false,
    },
    gem: {
      type: DataTypes.INTEGER,
      defaultValue: 0,
      allowNull: false,
    },
    created_at: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      allowNull: false,
    },
    last_login: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW,
      allowNull: false,
    },
  },
  {
    tableName: "user",
    timestamps: false,
  }
);

module.exports = User;
